using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;          // for DisplayNameAttribute
using System.Reflection;             // for reflection

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

// Drawing aliases for header image
using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;

using Microsoft.AspNetCore.Hosting;
using Nop.Data;

// ⬇️ adjust this to your actual domain namespace
using Nop.Plugin.Widgets.Outcome.Domain;
using Nop.Plugin.Widgets.Outcome.Models; // OutcomeModel constants + enums

namespace Nop.Plugin.Widgets.Outcome.Services
{
    public class OutcomeDocExportService : IOutcomeDocExportService
    {
        private readonly IRepository<OutcomeRecord> _outcomeRepo;
        private readonly IRepository<MentalHealthRecord> _mentalRepo;
        private readonly IWebHostEnvironment _env;

        private const string CategoryMentalHealthTitle = "Mental health & wellbeing";

        public OutcomeDocExportService(
            IRepository<OutcomeRecord> outcomeRepo,
            IRepository<MentalHealthRecord> mentalRepo,
            IWebHostEnvironment env)
        {
            _outcomeRepo = outcomeRepo;
            _mentalRepo = mentalRepo;
            _env = env;
        }

        public async Task<byte[]> BuildOutcomeDocAsync(int projectId, int outcomeId)
        {
            // LinqToDB IQueryable — use sync FirstOrDefault() to avoid EF deps
            var outcome = _outcomeRepo.Table
                .FirstOrDefault(o => o.Id == outcomeId && o.ProjectId == projectId);

            if (outcome is null)
                throw new InvalidOperationException("Outcome not found for the provided project/outcome id.");

            // Use the FK name on your entity; you indicated OutcomeRecordId
            var mental = _mentalRepo.Table
                .FirstOrDefault(m => m.OutcomeRecordId == outcomeId);

            using var ms = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                var main = doc.AddMainDocumentPart();
                main.Document = new Document(new Body());

                AddHeaderWithLogo(doc, TryGetLogoPath());
                AddFooter(doc, "© Social Research Builders Ltd · Company No. 15546161 · hello@socialresearchbuilders.com");

                var body = main.Document.Body!;

                // Title
                AppendTitle(body, "Outcome");

                // === Mental health & wellbeing ===
                WriteMentalSection(body, outcome, mental);

                main.Document.Save();
            }

            return ms.ToArray();
        }

        // ===================== Sections =====================

        private void WriteMentalSection(Body body, OutcomeRecord outcome, MentalHealthRecord? mental)
        {
            // Include section when Cat_MentalHealth flag is set on the outcome, or when any mental record exists
            var includeSection =
                ReadBoolFlag(outcome, "Cat_MentalHealth", "MentalHealth", "CatMentalHealth")
                || mental != null;

            if (!includeSection)
                return;

            AppendHeading(body, CategoryMentalHealthTitle, level: 1);

            if (mental == null)
                return;

            // ---- M1: title from OutcomeModel constant, render if selected or any field has data ----
            var m1Selected = ReadBoolFlag(mental, "M1_Selected");
            var m1Fields = new[] { "M1_Respondents", "M1_ReducedAnxietyCount" };

            if (m1Selected || HasAnyValue(mental, m1Fields))
            {
                AppendHeading(body, OutcomeModel.M1_Title, level: 2);

                foreach (var prop in m1Fields)
                {
                    var label = LabelFor(prop);   // uses [DisplayName] from OutcomeModel if set
                    AppendLabel(body, label);

                    var pi = mental.GetType().GetProperty(prop,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (pi == null)
                    {
                        AppendTextOrLine(body, null); // safety
                        continue;
                    }

                    var underlying = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                    var raw = pi.GetValue(mental);

                    if (underlying.IsEnum)
                    {
                        // Print a radio row for enum options (● selected, ○ unselected)
                        AppendEnumRadioRow(body, underlying, raw);
                    }
                    else
                    {
                        // Print value if present; otherwise an underline line
                        var value = GetValueAsString(mental, prop);
                        AppendTextOrLine(body, value, 32);
                    }
                }
            }

            // TODO: extend for M2..M15 using OutcomeModel.M*_Title and the same pattern
        }

        // ===================== DisplayName / labels =====================

        private static string LabelFor(string propName)
        {
            // Prefer [DisplayName("…")] from OutcomeModel wrapper; fallback to humanized prop
            var display = GetDisplayNameFromOutcomeModel(propName);
            return string.IsNullOrWhiteSpace(display) ? Humanize(propName) : display!;
        }

        private static string? GetDisplayNameFromOutcomeModel(string propName)
        {
            var pi = typeof(OutcomeModel).GetProperty(
                propName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (pi == null)
                return null;

            var attr = pi.GetCustomAttribute<DisplayNameAttribute>();
            return attr?.DisplayName;
        }

        private static string Humanize(string name)
        {
            // "M1_ReducedAnxietyCount" → "M1 Reduced Anxiety Count"
            var s = name.Replace('_', ' ');
            s = Regex.Replace(s, "([a-z])([A-Z])", "$1 $2");
            return s;
        }

        // ===================== Value helpers =====================

        private static bool HasAnyValue(object record, IEnumerable<string> propNames)
        {
            foreach (var name in propNames)
            {
                var v = GetValueAsString(record, name);
                if (!string.IsNullOrWhiteSpace(v))
                    return true;
            }
            return false;
        }

        private static bool ReadBoolFlag(object obj, params string[] names)
        {
            foreach (var n in names)
            {
                var p = obj.GetType().GetProperty(n,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (p == null)
                    continue;

                var v = p.GetValue(obj);
                if (v == null)
                    continue;

                switch (v)
                {
                    case bool b when b:
                        return true;
                    case byte by when by != 0:
                        return true;
                    case short sh when sh != 0:
                        return true;
                    case int i when i != 0:
                        return true;
                    case long l when l != 0:
                        return true;
                    case string s when s.Equals("true", StringComparison.OrdinalIgnoreCase)
                                    || s == "1"
                                    || s.Equals("yes", StringComparison.OrdinalIgnoreCase):
                        return true;
                }
            }
            return false;
        }

        private static string GetValueAsString(object obj, string propName)
        {
            var p = obj.GetType().GetProperty(propName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (p == null)
                return string.Empty;

            var v = p.GetValue(obj);
            if (v == null)
                return string.Empty;

            return v switch
            {
                DateTime dt => dt.ToString("yyyy-MM-dd"),
                _ => v.ToString() ?? string.Empty
            };
        }

        // ===================== Rendering helpers for placeholders =====================

        /// <summary>
        /// Prints the value if present, otherwise prints an underline line: "________________".
        /// </summary>
        private static void AppendTextOrLine(Body body, string? value, int lineLength = 32)
        {
            var toShow = string.IsNullOrWhiteSpace(value) ? new string('_', lineLength) : value!;
            var p = new Paragraph(new Run(new Text(toShow)));
            body.Append(p);
        }

        /// <summary>
        /// Prints enum choices as a single paragraph of "radio buttons" in a row.
        /// Selected = ●, Unselected = ○. Friendly option text is used where possible.
        /// </summary>
        private static void AppendEnumRadioRow(Body body, Type enumType, object? selectedValue)
        {
            var p = new Paragraph();

            var selectedName = selectedValue == null ? null : Enum.GetName(enumType, selectedValue);

            foreach (var option in Enum.GetValues(enumType))
            {
                var name = Enum.GetName(enumType, option);
                var isSelected = selectedName != null && string.Equals(name, selectedName, StringComparison.OrdinalIgnoreCase);

                var bullet = isSelected ? "●" : "○"; // filled / empty circle
                var label = GetEnumDisplayName(enumType, name!);

                // "● Yes" + spacing
                p.Append(new Run(new Text(bullet + " " + label)));

                // add a few spaces between options, preserved
                p.Append(new Run(new Text("   ") { Space = SpaceProcessingModeValues.Preserve }));
            }

            body.Append(p);
        }

        /// <summary>
        /// Friendly names for enum members. Uses special mapping for ReducedAnxietyLevel,
        /// then falls back to [Description] or PascalCase split.
        /// </summary>
        private static string GetEnumDisplayName(Type enumType, string memberName)
        {
            // Special-case mapping for ReducedAnxietyLevel (as requested)
            if (enumType == typeof(ReducedAnxietyLevel))
            {
                return memberName switch
                {
                    nameof(ReducedAnxietyLevel.Yes) => "Yes",
                    nameof(ReducedAnxietyLevel.ALot) => "A Lot",
                    nameof(ReducedAnxietyLevel.YesABit) => "Yes, A Bit",
                    _ => memberName
                };
            }

            // Try [Description] attribute if present
            var mi = enumType.GetMember(memberName).FirstOrDefault();
            var desc = mi?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                         .Cast<DescriptionAttribute>()
                         .FirstOrDefault()?.Description;
            if (!string.IsNullOrWhiteSpace(desc))
                return desc!;

            // Fallback: split PascalCase (e.g., "OneOffSurvey" -> "One Off Survey")
            var s = Regex.Replace(memberName, "([a-z])([A-Z])", "$1 $2");
            s = Regex.Replace(s, "([A-Z])([A-Z][a-z])", "$1 $2");
            return s;
        }

        // ===================== OpenXML helpers (kept in your style) =====================

        private static void AppendTitle(Body body, string text)
        {
            var pPr = new ParagraphProperties(
                new Justification { Val = JustificationValues.Left },
                new SpacingBetweenLines { After = "200" });

            var rPr = new RunProperties(new Bold(), new FontSize { Val = "36" });
            var run = new Run(rPr, new Text(text));

            var p = new Paragraph();
            p.Append(pPr);
            p.Append(run);

            body.Append(p);
        }

        private static void AppendHeading(Body body, string text, int level)
        {
            // Level 1 ~ 28pt, Level 2 ~ 24pt
            var size = level <= 1 ? "28" : "24";

            var pPr = new ParagraphProperties(new SpacingBetweenLines
            {
                Before = level <= 1 ? "200" : "120",
                After = level <= 1 ? "120" : "80"
            });

            var rPr = new RunProperties(new Bold(), new FontSize { Val = size });
            var run = new Run(rPr, new Text(text));

            var p = new Paragraph();
            p.Append(pPr);
            p.Append(run);

            body.Append(p);
        }

        private static void AppendLabel(Body body, string label)
        {
            var p = new Paragraph();
            p.Append(new Run(new RunProperties(new Bold()), new Text(label)));
            body.Append(p);
        }

        // (kept for compatibility; no longer used for textbox/enum placeholders)
        private static void AppendPlaceholderOrValue(Body body, string? value, string placeholder)
        {
            var toShow = string.IsNullOrWhiteSpace(value) ? placeholder : value;
            body.Append(new Paragraph(new Run(new Text(toShow))));
        }

        private static void AppendLabelValue(Body body, string label, string value)
        {
            var labelRun = new Run(new RunProperties(new Bold()), new Text(label + (string.IsNullOrWhiteSpace(value) ? "" : ":")));
            var spaceRun = new Run(new Text(" "));
            var valueRun = new Run(new Text(string.IsNullOrWhiteSpace(value) ? "" : value));

            var p = new Paragraph();
            p.Append(labelRun);
            p.Append(spaceRun);
            p.Append(valueRun);

            body.Append(p);
        }

        private static void AddFooter(WordprocessingDocument doc, string text)
        {
            var main = doc.MainDocumentPart!;
            var footerPart = main.AddNewPart<FooterPart>();
            var ftr = new Footer();

            var p = new Paragraph();
            p.Append(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
            p.Append(new Run(new Text(text)));

            ftr.Append(p);
            footerPart.Footer = ftr;

            var sectProps = main.Document.Body.Elements<SectionProperties>().LastOrDefault();
            if (sectProps == null)
            {
                sectProps = new SectionProperties();
                main.Document.Body.Append(sectProps);
            }

            sectProps.RemoveAllChildren<FooterReference>();
            sectProps.Append(new FooterReference { Type = HeaderFooterValues.Default, Id = main.GetIdOfPart(footerPart) });
        }

        private static void AddHeaderWithLogo(WordprocessingDocument doc, string? logoPath)
        {
            var main = doc.MainDocumentPart!;
            var headerPart = main.AddNewPart<HeaderPart>();
            var header = new Header();

            var p = new Paragraph();
            p.Append(new ParagraphProperties(new Justification { Val = JustificationValues.Right }));

            if (!string.IsNullOrWhiteSpace(logoPath))
            {
                var imgPart = headerPart.AddImagePart(ImagePartType.Png);
                using (var fs = File.OpenRead(logoPath))
                    imgPart.FeedData(fs);

                var relId = headerPart.GetIdOfPart(imgPart);
                var drawing = CreateInlineImage(relId, widthPx: 280, heightPx: 80);
                p.Append(new Run(drawing));
            }

            header.Append(p);
            headerPart.Header = header;

            var sectProps = main.Document.Body.Elements<SectionProperties>().LastOrDefault();
            if (sectProps == null)
            {
                sectProps = new SectionProperties();
                main.Document.Body.Append(sectProps);
            }

            sectProps.RemoveAllChildren<HeaderReference>();
            sectProps.Append(new HeaderReference { Type = HeaderFooterValues.Default, Id = main.GetIdOfPart(headerPart) });
        }

        private static Drawing CreateInlineImage(string relationshipId, long widthPx, long heightPx)
        {
            // EMUs conversion
            const long emusPerInch = 914400;
            const long pxPerInch = 96;
            long cx = widthPx * emusPerInch / pxPerInch;
            long cy = heightPx * emusPerInch / pxPerInch;

            var inline = new DW.Inline(
                new DW.Extent { Cx = cx, Cy = cy },
                new DW.EffectExtent { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                new DW.DocProperties { Id = (UInt32Value)1U, Name = "Impacto Logo" },
                new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks { NoChangeAspect = true }),
                new A.Graphic(new A.GraphicData(
                    new PIC.Picture(
                        new PIC.NonVisualPictureProperties(
                            new PIC.NonVisualDrawingProperties { Id = (UInt32Value)0U, Name = "logo.png" },
                            new PIC.NonVisualPictureDrawingProperties()),
                        new PIC.BlipFill(
                            new A.Blip { Embed = relationshipId },
                            new A.Stretch(new A.FillRectangle())),
                        new PIC.ShapeProperties(
                            new A.Transform2D(
                                new A.Offset { X = 0, Y = 0 },
                                new A.Extents { Cx = cx, Cy = cy }),
                            new A.PresetGeometry { Preset = A.ShapeTypeValues.Rectangle })))
                { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
            );

            inline.DistanceFromTop = 0U;
            inline.DistanceFromBottom = 0U;
            inline.DistanceFromLeft = 0U;
            inline.DistanceFromRight = 0U;

            return new Drawing(inline);
        }

        // ===================== paths =====================

        private string? TryGetLogoPath()
        {
            // Plugins/Widgets.Outcome/Content/Impacto_Logo.png
            var path = Path.Combine(_env.ContentRootPath, "Plugins", "Widgets.Outcome", "Content", "Impacto_Logo.png");
            return File.Exists(path) ? path : null;
        }
    }
}
