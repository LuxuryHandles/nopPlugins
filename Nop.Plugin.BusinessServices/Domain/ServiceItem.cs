using Nop.Core;
using System;

namespace Nop.Plugin.BusinessServices.Domain
{
    public class ServiceItem : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; } = true;
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
