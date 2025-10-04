using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class ServiceItemAdminController : BasePluginController
    {
        private readonly IBusinessCatalogService _svc;
        private const string VROOT = "~/Plugins/Nop.Plugin.BusinessServices/Views/";

        public ServiceItemAdminController(IBusinessCatalogService svc) => _svc = svc;

        public async Task<IActionResult> List(string q = null)
        {
            // load types, services, and all mappings in one go
            var types = await _svc.GetAllBusinessTypesAsync(null, null);
            var services = await _svc.GetAllServiceItemsAsync(q, null);
            var mappings = await _svc.GetAllMappingsAsync();

            // index mappings by ServiceItemId -> { BusinessTypeId, ... }
            var mapByService = mappings
                .GroupBy(t => t.ServiceItemId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.BusinessTypeId).ToHashSet());

            var vm = new ServiceListViewModel
            {
                Types = types.Select(t => new BusinessTypeModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Published = t.Published
                }).ToList(),
                Services = services.Select(s => new ServiceListRowModel
                {
                    Service = new ServiceItemModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        Published = s.Published
                    },
                    TypeIds = mapByService.TryGetValue(s.Id, out var set) ? set : new HashSet<int>()
                }).ToList()
            };

            return View(VROOT + "ServiceItem/List.cshtml", vm);
        }


        [HttpGet]
        public IActionResult Create() => View(VROOT + "ServiceItem/Create.cshtml", new ServiceItemModel());

        [HttpPost]
        public async Task<IActionResult> Create(ServiceItemModel model)
        {
            if (!ModelState.IsValid)
                return View(VROOT + "ServiceItem/Create.cshtml", model);

            await _svc.InsertServiceItemAsync(new Domain.ServiceItem
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
            var s = await _svc.GetServiceItemByIdAsync(id);
            if (s == null)
                return RedirectToAction(nameof(List));

            var model = new ServiceItemModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Published = s.Published
            };
            return View(VROOT + "ServiceItem/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceItemModel model)
        {
            if (!ModelState.IsValid)
                return View(VROOT + "ServiceItem/Edit.cshtml", model);

            var s = await _svc.GetServiceItemByIdAsync(model.Id);
            if (s == null)
                return RedirectToAction(nameof(List));

            s.Name = model.Name;
            s.Description = model.Description;
            s.Published = model.Published;
            await _svc.UpdateServiceItemAsync(s);
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _svc.GetServiceItemByIdAsync(id);
            if (s != null)
                await _svc.DeleteServiceItemAsync(s);
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> SeedDefaults()
        {
            await _svc.SeedDefaultServicesAsync();
            return RedirectToAction(nameof(List));
        }
    }
}
