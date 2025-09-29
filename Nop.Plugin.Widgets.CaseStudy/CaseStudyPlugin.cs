using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Cms;               // IWidgetPlugin
using Nop.Services.Plugins;           // BasePlugin
using Nop.Web.Framework.Infrastructure; // PublicWidgetZones (optional)

namespace Nop.Plugin.Widgets.CaseStudy
{
    public class CaseStudyPlugin : BasePlugin, IWidgetPlugin
    {
        // Show in Admin > Configuration > Widgets list
        public bool HideInWidgetList => false;

        // Which zones you want to render in (optional if you don't use zones)
        public Task<IList<string>> GetWidgetZonesAsync() =>
            Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.AccountNavigationAfter
            });

        // Return the TYPE of your ViewComponent (not the name string)
        public Type GetWidgetViewComponent(string widgetZone) =>
            typeof(Components.CaseStudyNavLinkViewComponent);

        public override Task InstallAsync() => base.InstallAsync();
        public override Task UninstallAsync() => base.UninstallAsync();
    }
}
