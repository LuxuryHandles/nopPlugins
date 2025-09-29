// /Plugins/Widgets.Outcome/Controllers/OutcomeController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Widgets.Outcome.Domain;
using Nop.Plugin.Widgets.Outcome.Models;
using Nop.Plugin.Widgets.Outcome.Services;

namespace Nop.Plugin.Widgets.Outcome.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class OutcomeController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IOutcomeService _outcomeService;
        private readonly IMentalHealthService _mentalService;
        private readonly IOutcomeStepsService _stepsService;
        private readonly IOutcomeDocExportService _exportService;

        public OutcomeController(
            IWorkContext workContext,
            IOutcomeService outcomeService,
            IMentalHealthService mentalService,
            IOutcomeStepsService stepsService,
            IOutcomeDocExportService exportService)
        {
            _workContext = workContext;
            _outcomeService = outcomeService;
            _mentalService = mentalService;
            _stepsService = stepsService;
            _exportService = exportService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? projectId, int? outcomeId)
        {
            if (projectId is null or <= 0)
                return BadRequest("ProjectID is required.");

            var customerId = (await _workContext.GetCurrentCustomerAsync()).Id;

            // If outcomeId supplied, load master + mental and render
            if (outcomeId is > 0)
            {
                var master = await _outcomeService.GetByIdForCustomerAsync(outcomeId.Value, customerId);
                if (master == null || master.ProjectId != projectId.Value)
                {
                    TempData["OutcomeMessage"] = "We couldn't find that outcome for this project.";
                    TempData["OutcomeMessageType"] = "error";
                    return RedirectToAction("Index", new { projectId });
                }

                var mental = await _mentalService.GetByOutcomeIdAsync(master.Id, customerId) ?? new MentalHealthRecord
                {
                    OutcomeRecordId = master.Id,
                    ProjectId = master.ProjectId,
                    CustomerId = master.CustomerId
                };

                var vm = ToModel(master, mental);
                vm.Existing = true;
                vm.InfoMessage = TempData["OutcomeMessage"] as string;
                vm.InfoMessageType = TempData["OutcomeMessageType"] as string;

                return View("~/Plugins/Widgets.Outcome/Views/Outcome/Index.cshtml", vm);
            }

            // No outcomeId: try follow Steps link first (keep your existing pattern)
            var linkedId = await _stepsService.GetOutcomeIdAsync(customerId, projectId.Value);
            if (linkedId is > 0)
                return RedirectToAction("Index", new { projectId, outcomeId = linkedId });

            // brand-new
            var blank = new OutcomeModel
            {
                ProjectId = projectId.Value,
                Existing = false,
                InfoMessage = TempData["OutcomeMessage"] as string,
                InfoMessageType = TempData["OutcomeMessageType"] as string
            };
            return View("~/Plugins/Widgets.Outcome/Views/Outcome/Index.cshtml", blank);
        }

        [HttpPost]
        public async Task<IActionResult> Save(OutcomeModel model)
        {
            var customerId = (await _workContext.GetCurrentCustomerAsync()).Id;

            // Normalize Mental inputs based on selected subcategories
            SanitizeMental(model.Mental);

            if (!ModelState.IsValid)
            {
                model.Existing = model.Id > 0;
                model.InfoMessage = "Please correct the validation errors.";
                model.InfoMessageType = "error";
                return View("~/Plugins/Widgets.Outcome/Views/Outcome/Index.cshtml", model);
            }

            OutcomeRecord master;

            if (model.Id <= 0)
            {
                // CREATE master
                master = new OutcomeRecord
                {
                    CustomerId = customerId,
                    ProjectId = model.ProjectId,
                    Cat_MentalHealth = model.Cat_MentalHealth || AnyMentalSelected(model.Mental),
                    Cat_PhysicalHealth = model.Cat_PhysicalHealth,
                    Cat_Employment = model.Cat_Employment,
                    Cat_SocialConnection = model.Cat_SocialConnection,
                    Cat_Culture = model.Cat_Culture,
                    Cat_Housing = model.Cat_Housing,
                    Cat_Religious = model.Cat_Religious,
                    Cat_Environment = model.Cat_Environment,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };

                await _outcomeService.InsertAsync(master);
            }
            else
            {
                // UPDATE master
                master = await _outcomeService.GetByIdForCustomerAsync(model.Id, customerId)
                    ?? throw new InvalidOperationException("Outcome master not found");
                master.Cat_MentalHealth = model.Cat_MentalHealth || AnyMentalSelected(model.Mental);
                master.Cat_PhysicalHealth = model.Cat_PhysicalHealth;
                master.Cat_Employment = model.Cat_Employment;
                master.Cat_SocialConnection = model.Cat_SocialConnection;
                master.Cat_Culture = model.Cat_Culture;
                master.Cat_Housing = model.Cat_Housing;
                master.Cat_Religious = model.Cat_Religious;
                master.Cat_Environment = model.Cat_Environment;
                master.UpdatedOnUtc = DateTime.UtcNow;

                await _outcomeService.UpdateAsync(master);
            }

            // Upsert Mental (persist selected flags even when no inputs)
            var mentalDb = await _mentalService.GetByOutcomeIdAsync(master.Id, customerId);
            if (mentalDb == null)
            {
                mentalDb = FromMentalModel(model.Mental, new MentalHealthRecord
                {
                    OutcomeRecordId = master.Id,
                    CustomerId = customerId,
                    ProjectId = master.ProjectId
                });
                await _mentalService.InsertAsync(mentalDb);
            }
            else
            {
                mentalDb = FromMentalModel(model.Mental, mentalDb);
                await _mentalService.UpdateAsync(mentalDb);
            }

            // Update steps (link outcome + set Step3=1)
            await _stepsService.UpdateOutcomeLinkAndStep3OnlyAsync(customerId, master.ProjectId, master.Id);

            TempData["OutcomeMessage"] = model.Id <= 0 ? "Outcome created." : "Outcome updated.";
            TempData["OutcomeMessageType"] = "success";
            return RedirectToAction("Index", new { projectId = master.ProjectId, outcomeId = master.Id });
        }

        // --- mapping
        private static OutcomeModel ToModel(OutcomeRecord master, MentalHealthRecord mental)
        {
            var vm = new OutcomeModel
            {
                Id = master.Id,
                ProjectId = master.ProjectId,
                Cat_MentalHealth = master.Cat_MentalHealth,
                Cat_PhysicalHealth = master.Cat_PhysicalHealth,
                Cat_Employment = master.Cat_Employment,
                Cat_SocialConnection = master.Cat_SocialConnection,
                Cat_Culture = master.Cat_Culture,
                Cat_Housing = master.Cat_Housing,
                Cat_Religious = master.Cat_Religious,
                Cat_Environment = master.Cat_Environment,
                Mental = new MentalHealthModel
                {
                    Id = mental.Id,
                    OutcomeRecordId = mental.OutcomeRecordId,
                    ProjectId = mental.ProjectId,

                    M1_Selected = mental.M1_Selected,
                    M2_Selected = mental.M2_Selected,
                    M3_Selected = mental.M3_Selected,
                    M4_Selected = mental.M4_Selected,
                    M5_Selected = mental.M5_Selected,
                    M6_Selected = mental.M6_Selected,
                    M7_Selected = mental.M7_Selected,
                    M8_Selected = mental.M8_Selected,
                    M9_Selected = mental.M9_Selected,
                    M10_Selected = mental.M10_Selected,
                    M11_Selected = mental.M11_Selected,
                    M12_Selected = mental.M12_Selected,
                    M13_Selected = mental.M13_Selected,
                    M14_Selected = mental.M14_Selected,
                    M15_Selected = mental.M15_Selected,

                    M1_Respondents = mental.M1_Respondents,
                    M1_ReducedAnxietyCount = mental.M1_ReducedAnxietyCount,

                    M2_Respondents = mental.M2_Respondents,
                    M2_SurveyType = mental.M2_SurveyType,
                    M2_OneOff_Improved6mCount = mental.M2_OneOff_Improved6mCount,
                    M2_PrePost_AvgImprovement = mental.M2_PrePost_AvgImprovement,

                    M3_Respondents = mental.M3_Respondents,
                    M3_SurveyType = mental.M3_SurveyType,
                    M3_OneOff_Improved6mCount = mental.M3_OneOff_Improved6mCount,
                    M3_PrePost_AvgImprovement = mental.M3_PrePost_AvgImprovement,

                    M4_ParticipantsTotal = mental.M4_ParticipantsTotal,
                    M4_FirstTimeParticipants = mental.M4_FirstTimeParticipants,
                    M4_RepeatParticipants = mental.M4_RepeatParticipants,

                    M5_Respondents = mental.M5_Respondents,
                    M5_SurveyType = mental.M5_SurveyType,
                    M5_OneOff_Improved6mCount = mental.M5_OneOff_Improved6mCount,
                    M5_PrePost_AvgImprovement = mental.M5_PrePost_AvgImprovement,

                    M6_Respondents = mental.M6_Respondents,
                    M6_SurveyType = mental.M6_SurveyType,
                    M6_OneOff_Improved6mCount = mental.M6_OneOff_Improved6mCount,
                    M6_PrePost_AvgImprovement = mental.M6_PrePost_AvgImprovement,

                    M7_GroupParticipantsTotal = mental.M7_GroupParticipantsTotal,
                    M7_GroupActivitiesPerWeek = mental.M7_GroupActivitiesPerWeek,
                    M7_ReferralNetworks = mental.M7_ReferralNetworks,

                    M8_Respondents = mental.M8_Respondents,
                    M8_SurveyType = mental.M8_SurveyType,
                    M8_OneOff_Improved6mCount = mental.M8_OneOff_Improved6mCount,
                    M8_PrePost_AvgImprovement = mental.M8_PrePost_AvgImprovement,

                    M9_Respondents = mental.M9_Respondents,
                    M9_ManagedToHandleMentalHealthCount = mental.M9_ManagedToHandleMentalHealthCount,

                    M10_FewerCrisisIncidentsCount = mental.M10_FewerCrisisIncidentsCount,
                    M11_UrgentReferralsReductionPct = mental.M11_UrgentReferralsReductionPct,

                    M12_RespondedCount = mental.M12_RespondedCount,
                    M12_BetterAwarenessCount = mental.M12_BetterAwarenessCount,

                    M13_LeaderObservations = mental.M13_LeaderObservations,

                    M14_Respondents = mental.M14_Respondents,
                    M14_SurveyType = mental.M14_SurveyType,
                    M14_OneOff_Improved6mCount = mental.M14_OneOff_Improved6mCount,
                    M14_PrePost_AvgImprovement = mental.M14_PrePost_AvgImprovement,

                    M15_ProgrammeCompletionsCount = mental.M15_ProgrammeCompletionsCount,
                    M15_NoRelapse6mPct = mental.M15_NoRelapse6mPct
                }
            };

            // If any mental subcategory has data, ensure top-level is checked
            if (!vm.Cat_MentalHealth)
                vm.Cat_MentalHealth = AnyMentalSelected(vm.Mental);

            return vm;
        }

        private static MentalHealthRecord FromMentalModel(MentalHealthModel m, MentalHealthRecord e)
        {
            // Always persist selected flags even if inputs remain null
            e.M1_Selected = m.M1_Selected;
            e.M2_Selected = m.M2_Selected;
            e.M3_Selected = m.M3_Selected;
            e.M4_Selected = m.M4_Selected;
            e.M5_Selected = m.M5_Selected;
            e.M6_Selected = m.M6_Selected;
            e.M7_Selected = m.M7_Selected;
            e.M8_Selected = m.M8_Selected;
            e.M9_Selected = m.M9_Selected;
            e.M10_Selected = m.M10_Selected;
            e.M11_Selected = m.M11_Selected;
            e.M12_Selected = m.M12_Selected;
            e.M13_Selected = m.M13_Selected;
            e.M14_Selected = m.M14_Selected;
            e.M15_Selected = m.M15_Selected;

            e.M1_Respondents = m.M1_Respondents;
            e.M1_ReducedAnxietyCount = m.M1_ReducedAnxietyCount;

            e.M2_Respondents = m.M2_Respondents;
            e.M2_SurveyType = m.M2_SurveyType;
            e.M2_OneOff_Improved6mCount = m.M2_OneOff_Improved6mCount;
            e.M2_PrePost_AvgImprovement = m.M2_PrePost_AvgImprovement;

            e.M3_Respondents = m.M3_Respondents;
            e.M3_SurveyType = m.M3_SurveyType;
            e.M3_OneOff_Improved6mCount = m.M3_OneOff_Improved6mCount;
            e.M3_PrePost_AvgImprovement = m.M3_PrePost_AvgImprovement;

            e.M4_ParticipantsTotal = m.M4_ParticipantsTotal;
            e.M4_FirstTimeParticipants = m.M4_FirstTimeParticipants;
            e.M4_RepeatParticipants = m.M4_RepeatParticipants;

            e.M5_Respondents = m.M5_Respondents;
            e.M5_SurveyType = m.M5_SurveyType;
            e.M5_OneOff_Improved6mCount = m.M5_OneOff_Improved6mCount;
            e.M5_PrePost_AvgImprovement = m.M5_PrePost_AvgImprovement;

            e.M6_Respondents = m.M6_Respondents;
            e.M6_SurveyType = m.M6_SurveyType;
            e.M6_OneOff_Improved6mCount = m.M6_OneOff_Improved6mCount;
            e.M6_PrePost_AvgImprovement = m.M6_PrePost_AvgImprovement;

            e.M7_GroupParticipantsTotal = m.M7_GroupParticipantsTotal;
            e.M7_GroupActivitiesPerWeek = m.M7_GroupActivitiesPerWeek;
            e.M7_ReferralNetworks = m.M7_ReferralNetworks;

            e.M8_Respondents = m.M8_Respondents;
            e.M8_SurveyType = m.M8_SurveyType;
            e.M8_OneOff_Improved6mCount = m.M8_OneOff_Improved6mCount;
            e.M8_PrePost_AvgImprovement = m.M8_PrePost_AvgImprovement;

            e.M9_Respondents = m.M9_Respondents;
            e.M9_ManagedToHandleMentalHealthCount = m.M9_ManagedToHandleMentalHealthCount;

            e.M10_FewerCrisisIncidentsCount = m.M10_FewerCrisisIncidentsCount;
            e.M11_UrgentReferralsReductionPct = m.M11_UrgentReferralsReductionPct;

            e.M12_RespondedCount = m.M12_RespondedCount;
            e.M12_BetterAwarenessCount = m.M12_BetterAwarenessCount;

            e.M13_LeaderObservations = m.M13_LeaderObservations;

            e.M14_Respondents = m.M14_Respondents;
            e.M14_SurveyType = m.M14_SurveyType;
            e.M14_OneOff_Improved6mCount = m.M14_OneOff_Improved6mCount;
            e.M14_PrePost_AvgImprovement = m.M14_PrePost_AvgImprovement;

            e.M15_ProgrammeCompletionsCount = m.M15_ProgrammeCompletionsCount;
            e.M15_NoRelapse6mPct = m.M15_NoRelapse6mPct;

            return e;
        }

        private static bool AnyMentalSelected(MentalHealthModel m)
            => m.M1_Selected || m.M2_Selected || m.M3_Selected || m.M4_Selected || m.M5_Selected ||
               m.M6_Selected || m.M7_Selected || m.M8_Selected || m.M9_Selected || m.M10_Selected ||
               m.M11_Selected || m.M12_Selected || m.M13_Selected || m.M14_Selected || m.M15_Selected;

        private static void SanitizeMental(MentalHealthModel m)
        {
            // We persist *_Selected flags even when inputs are empty.
            // When a subcategory is NOT selected, null out its inputs.

            if (!m.M1_Selected)
            { m.M1_Respondents = null; m.M1_ReducedAnxietyCount = null; }

            if (!m.M2_Selected)
            { m.M2_Respondents = null; m.M2_SurveyType = null; m.M2_OneOff_Improved6mCount = null; m.M2_PrePost_AvgImprovement = null; }
            else
            {
                if (m.M2_SurveyType == null)
                { m.M2_OneOff_Improved6mCount = null; m.M2_PrePost_AvgImprovement = null; }
                else if (m.M2_SurveyType == SurveyType.OneOffSurvey)
                { m.M2_PrePost_AvgImprovement = null; }
                else
                { m.M2_OneOff_Improved6mCount = null; }
            }

            if (!m.M3_Selected)
            { m.M3_Respondents = null; m.M3_SurveyType = null; m.M3_OneOff_Improved6mCount = null; m.M3_PrePost_AvgImprovement = null; }
            else
            {
                if (m.M3_SurveyType == null)
                { m.M3_OneOff_Improved6mCount = null; m.M3_PrePost_AvgImprovement = null; }
                else if (m.M3_SurveyType == SurveyType.OneOffSurvey)
                { m.M3_PrePost_AvgImprovement = null; }
                else
                { m.M3_OneOff_Improved6mCount = null; }
            }

            if (!m.M4_Selected)
            { m.M4_ParticipantsTotal = null; m.M4_FirstTimeParticipants = null; m.M4_RepeatParticipants = null; }

            if (!m.M5_Selected)
            { m.M5_Respondents = null; m.M5_SurveyType = null; m.M5_OneOff_Improved6mCount = null; m.M5_PrePost_AvgImprovement = null; }
            else
            {
                if (m.M5_SurveyType == null)
                { m.M5_OneOff_Improved6mCount = null; m.M5_PrePost_AvgImprovement = null; }
                else if (m.M5_SurveyType == SurveyType.OneOffSurvey)
                { m.M5_PrePost_AvgImprovement = null; }
                else
                { m.M5_OneOff_Improved6mCount = null; }
            }

            if (!m.M6_Selected)
            { m.M6_Respondents = null; m.M6_SurveyType = null; m.M6_OneOff_Improved6mCount = null; m.M6_PrePost_AvgImprovement = null; }
            else
            {
                if (m.M6_SurveyType == null)
                { m.M6_OneOff_Improved6mCount = null; m.M6_PrePost_AvgImprovement = null; }
                else if (m.M6_SurveyType == SurveyType.OneOffSurvey)
                { m.M6_PrePost_AvgImprovement = null; }
                else
                { m.M6_OneOff_Improved6mCount = null; }
            }

            if (!m.M7_Selected)
            { m.M7_GroupParticipantsTotal = null; m.M7_GroupActivitiesPerWeek = null; m.M7_ReferralNetworks = null; }

            if (!m.M8_Selected)
            { m.M8_Respondents = null; m.M8_SurveyType = null; m.M8_OneOff_Improved6mCount = null; m.M8_PrePost_AvgImprovement = null; }
            else
            {
                if (m.M8_SurveyType == null)
                { m.M8_OneOff_Improved6mCount = null; m.M8_PrePost_AvgImprovement = null; }
                else if (m.M8_SurveyType == SurveyType.OneOffSurvey)
                { m.M8_PrePost_AvgImprovement = null; }
                else
                { m.M8_OneOff_Improved6mCount = null; }
            }

            if (!m.M9_Selected)
            { m.M9_Respondents = null; m.M9_ManagedToHandleMentalHealthCount = null; }
            if (!m.M10_Selected)
            { m.M10_FewerCrisisIncidentsCount = null; }
            if (!m.M11_Selected)
            { m.M11_UrgentReferralsReductionPct = null; }
            if (!m.M12_Selected)
            { m.M12_RespondedCount = null; m.M12_BetterAwarenessCount = null; }
            if (!m.M13_Selected)
            { m.M13_LeaderObservations = null; }

            if (!m.M14_Selected)
            { m.M14_Respondents = null; m.M14_SurveyType = null; m.M14_OneOff_Improved6mCount = null; m.M14_PrePost_AvgImprovement = null; }
            else
            {
                if (m.M14_SurveyType == null)
                { m.M14_OneOff_Improved6mCount = null; m.M14_PrePost_AvgImprovement = null; }
                else if (m.M14_SurveyType == SurveyType.OneOffSurvey)
                { m.M14_PrePost_AvgImprovement = null; }
                else
                { m.M14_OneOff_Improved6mCount = null; }
            }

            if (!m.M15_Selected)
            { m.M15_ProgrammeCompletionsCount = null; m.M15_NoRelapse6mPct = null; }
        }

        [HttpGet]
        public async Task<IActionResult> ResearchTools(int projectId, int outcomeId)
        {
            try
            {
                var bytes = await _exportService.BuildOutcomeDocAsync(projectId, outcomeId);
                var fileName = $"Impacto-Outcome-{projectId}-{outcomeId}.docx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                // You can log ex here via your logger
                return BadRequest(ex.Message);
            }
        }

    }
}
