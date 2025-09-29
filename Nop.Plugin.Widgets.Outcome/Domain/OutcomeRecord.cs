using Nop.Core;

namespace Nop.Plugin.Widgets.Outcome.Domain
{
    /// <summary>
    /// Master record for a project's Outcomes. Only holds top-level category flags.
    /// </summary>
    public class OutcomeRecord : BaseEntity
    {
        public int CustomerId { get; set; }
        public int ProjectId { get; set; }

        // Top-level categories
        public bool Cat_MentalHealth { get; set; }
        public bool Cat_PhysicalHealth { get; set; }
        public bool Cat_Employment { get; set; }
        public bool Cat_SocialConnection { get; set; }
        public bool Cat_Culture { get; set; }
        public bool Cat_Housing { get; set; }
        public bool Cat_Religious { get; set; }
        public bool Cat_Environment { get; set; }

        public System.DateTime CreatedOnUtc { get; set; }
        public System.DateTime UpdatedOnUtc { get; set; }
    }
}
