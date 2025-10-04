using System.Collections.Generic;

namespace Nop.Plugin.BusinessServices.Models
{
    public class ServiceListViewModel
    {
        public IList<BusinessTypeModel> Types { get; set; } = new List<BusinessTypeModel>();
        public IList<ServiceListRowModel> Services { get; set; } = new List<ServiceListRowModel>();
    }

    public class ServiceListRowModel
    {
        public ServiceItemModel Service { get; set; }
        // BusinessTypeIds this service is mapped to
        public HashSet<int> TypeIds { get; set; } = new HashSet<int>();
    }
}
