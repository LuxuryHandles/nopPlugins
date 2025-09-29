using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Outcome.Components
{
    public class OutcomeViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(string widgetZone, object additionalData) => Content(string.Empty);
    }
}
