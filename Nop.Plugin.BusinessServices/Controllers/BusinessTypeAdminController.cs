using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.BusinessServices.Domain;
using Nop.Plugin.BusinessServices.Models;
using Nop.Plugin.BusinessServices.Services;

namespace Nop.Plugin.BusinessServices.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    public class BusinessTypeAdminController : BasePluginController
    {
        private readonly IBusinessCatalogService _svc;
        private const string VROOT = "~/Plugins/Nop.Plugin.BusinessServices/Views/";

        public BusinessTypeAdminController(IBusinessCatalogService svc)
        {
            _svc = svc;
        }

        public async Task<IActionResult> List(string q = null)
        {
            var items = await _svc.GetAllBusinessTypesAsync(q, null);
            var model = items.Select(x => new BusinessTypeModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Published = x.Published
            }).ToList();

            return View(VROOT + "BusinessType/List.cshtml", model);
        }

        [HttpGet]
        public IActionResult Create() => View(VROOT + "BusinessType/Create.cshtml", new BusinessTypeModel());

        [HttpPost]
        public async Task<IActionResult> Create(BusinessTypeModel model)
        {
            if (!ModelState.IsValid)
                return View(VROOT + "BusinessType/Create.cshtml", model);

            await _svc.InsertBusinessTypeAsync(new BusinessType
            {
                Name = model.Name,
                Description = model.Description,
                Published = model.Published
            });
            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var e = await _svc.GetBusinessTypeByIdAsync(id);
            if (e == null)
                return RedirectToAction(nameof(List));

            var model = new BusinessTypeModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Published = e.Published
            };
            return View(VROOT + "BusinessType/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BusinessTypeModel model)
        {
            if (!ModelState.IsValid)
                return View(VROOT + "BusinessType/Edit.cshtml", model);

            var e = await _svc.GetBusinessTypeByIdAsync(model.Id);
            if (e == null)
                return RedirectToAction(nameof(List));

            e.Name = model.Name;
            e.Description = model.Description;
            e.Published = model.Published;
            await _svc.UpdateBusinessTypeAsync(e);
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _svc.GetBusinessTypeByIdAsync(id);
            if (e != null)
                await _svc.DeleteBusinessTypeAsync(e);
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> SeedDefaults()
        {
            await _svc.SeedDefaultBusinessTypesAsync();
            return RedirectToAction(nameof(List));
        }

        // NEW: One-click seed (types + services + mapping)
        [HttpPost]
        public async Task<IActionResult> SeedAll()
        {
            await _svc.SeedAllWithMappingsAsync();
            return RedirectToAction(nameof(List));
        }
    }
}
