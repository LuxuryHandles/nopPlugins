using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.CaseStudy.Infrastructure
{
    public class CaseStudyRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapControllerRoute(
                name: "Plugin.Widgets.CaseStudy.Index",
                pattern: "casestudy",
                defaults: new { controller = "CaseStudy", action = "Index" });
        }

        public int Priority => 0;
    }
}
