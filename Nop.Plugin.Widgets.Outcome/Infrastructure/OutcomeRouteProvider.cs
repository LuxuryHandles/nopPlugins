using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.Outcome.Infrastructure
{
    public class OutcomeRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapControllerRoute(
                name: "Plugin.Widgets.Outcome.Outcome",
                pattern: "Outcome", // /Outcome?projectid=... [&outcomeid=...]
                defaults: new { controller = "Outcome", action = "Index" });

            routeBuilder.MapControllerRoute(
                name: "Plugin.Widgets.Outcome.ResearchTools",
                pattern: "Outcome/ResearchTools",
                defaults: new { controller = "Outcome", action = "ResearchTools" }
            );
        }
        public int Priority => 0;
    }
}
