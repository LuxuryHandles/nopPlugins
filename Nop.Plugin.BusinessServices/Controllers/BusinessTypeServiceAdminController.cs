using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.BusinessServices.Models;
using Nop.Plugin.BusinessServices.Services;

namespace Nop.Plugin.BusinessServices.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    public class BusinessTypeServiceAdminController : BasePluginController
    {
        private readonly IBusinessCatalogService _svc;
        private const string VROOT = "~/Plugins/Nop.Plugin.BusinessServices/Views/";

        public BusinessTypeServiceAdminController(IBusinessCatalogService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> Map(int businessTypeId)
        {
            var bt = await _svc.GetBusinessTypeByIdAsync(businessTypeId);
            if (bt == null)
                return RedirectToAction("List", "BusinessTypeAdmin");

            var allServices = await _svc.GetAllServiceItemsAsync(null, true);
            var selected = (await _svc.GetServicesForBusinessTypeAsync(businessTypeId)).Select(s => s.Id).ToList();

            var model = new BusinessTypeServiceMapModel
            {
                BusinessTypeId = businessTypeId,
                BusinessTypeName = bt.Name,
                SelectedServiceIds = selected,
                AllServices = allServices.Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList()
            };

            return View(VROOT + "BusinessTypeService/Map.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Map(BusinessTypeServiceMapModel model)
        {
            await _svc.MapServicesToBusinessTypeAsync(model.BusinessTypeId, model.SelectedServiceIds);
            return RedirectToAction(nameof(Map), new { businessTypeId = model.BusinessTypeId });
        }
    }
}
