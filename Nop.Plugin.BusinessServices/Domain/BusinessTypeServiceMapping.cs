using Nop.Core;
using System;

namespace Nop.Plugin.BusinessServices.Domain
{
    public class BusinessTypeServiceMapping : BaseEntity
    {
        public int BusinessTypeId { get; set; }
        public int ServiceItemId { get; set; }
        public bool Published { get; set; } = true;
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
