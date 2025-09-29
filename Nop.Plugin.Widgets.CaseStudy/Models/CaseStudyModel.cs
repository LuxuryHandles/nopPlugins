using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.CaseStudy.Models
{
    public class CaseStudyModel
    {
        public int Id { get; set; }            // CaseStudyRecord Id
        public int ProjectId { get; set; }     // required in querystring
        public bool Existing { get; set; }

        [UIHint("Textarea")]
        public string CaseStudy { get; set; }
    }
}
