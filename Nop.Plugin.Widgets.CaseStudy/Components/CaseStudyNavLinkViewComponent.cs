using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.CaseStudy.Components
{
    public class CaseStudyNavLinkViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;

        public CaseStudyNavLinkViewComponent(IWorkContext workContext, ICustomerService customerService)
        {
            _workContext = workContext;
            _customerService = customerService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Content(string.Empty);

            var roles = await _customerService.GetCustomerRolesAsync(customer);
            var isRegistered = roles.Any(r => r.SystemName == NopCustomerDefaults.RegisteredRoleName);
            if (!isRegistered)
                return Content(string.Empty);

            return View("~/Plugins/Widgets.CaseStudy/Views/Shared/Components/CaseStudyNavLink/Default.cshtml");
        }
    }
}
