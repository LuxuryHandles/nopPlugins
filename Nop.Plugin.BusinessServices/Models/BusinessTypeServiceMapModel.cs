using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.BusinessServices.Models
{
    public class BusinessTypeServiceMapModel
    {
        public int BusinessTypeId { get; set; }
        public string BusinessTypeName { get; set; }

        public IList<int> SelectedServiceIds { get; set; } = new List<int>();
        public IList<SelectListItem> AllServices { get; set; } = new List<SelectListItem>();
    }
}
