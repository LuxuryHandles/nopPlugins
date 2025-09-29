using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Outcome
{
    public class OutcomePlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;
        public override string GetConfigurationPageUrl() => string.Empty;

        public System.Type GetWidgetViewComponent(string widgetZone)
            => typeof(Components.OutcomeViewComponent);

        public Task<IList<string>> GetWidgetZonesAsync()
            => Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.BodyEndHtmlTagBefore });

        public override Task InstallAsync() => base.InstallAsync();
        public override Task UninstallAsync() => base.UninstallAsync();
    }
}
