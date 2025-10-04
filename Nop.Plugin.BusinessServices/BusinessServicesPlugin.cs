using System.Threading.Tasks;
using Nop.Services.Events;
using Nop.Services.Plugins;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.BusinessServices
{
    public class BusinessServicesPlugin : BasePlugin, IConsumer<AdminMenuCreatedEvent>
    {
        public override string GetConfigurationPageUrl() => string.Empty;

        public Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
        {
            var root = new AdminMenuItem
            {
                SystemName = "BusinessServices.Root",
                Title = "Business Services",
                IconClass = "far fa-building",
                Visible = true
            };

            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "BusinessServices.Types",
                Title = "Business Types",
                Url = eventMessage.GetMenuItemUrl("BusinessTypeAdmin", "List"),
                Visible = true
            });

            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "BusinessServices.Services",
                Title = "Services",
                Url = eventMessage.GetMenuItemUrl("ServiceItemAdmin", "List"),
                Visible = true
            });

            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "BusinessServices.Mapping",
                Title = "Map Services to Type",
                Url = eventMessage.GetMenuItemUrl("BusinessTypeAdmin", "List"),
                Visible = true
            });

            eventMessage.RootMenuItem.ChildNodes.Add(root);
            return Task.CompletedTask;
        }

        public override Task InstallAsync() => base.InstallAsync();
        public override Task UninstallAsync() => base.UninstallAsync();
    }
}
