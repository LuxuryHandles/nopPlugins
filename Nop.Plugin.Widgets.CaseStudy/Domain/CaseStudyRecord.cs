using System;
using Nop.Core;

namespace Nop.Plugin.Widgets.CaseStudy.Domain
{
    public class CaseStudyRecord : BaseEntity
    {
        public int CustomerId { get; set; }
        public int ProjectId { get; set; }

        public string CaseStudy { get; set; } // nvarchar(MAX)

        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
