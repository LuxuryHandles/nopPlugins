using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CaseStudy.Services;
using Nop.Web.Framework.Infrastructure; // INopStartup

namespace Nop.Plugin.Widgets.CaseStudy.Infrastructure
{
    // MUST be public so nop can discover it
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register your services so controllers can resolve them
            services.AddScoped<ICaseStudyService, CaseStudyService>();
            services.AddScoped<ICaseStudyStepsService, CaseStudyStepsService>();
        }

        // No middleware needed for this plugin
        public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder application)
        {
        }

        // Order can be any number; keep it after core (>= 100 is safe)
        public int Order => 100;
    }
}
