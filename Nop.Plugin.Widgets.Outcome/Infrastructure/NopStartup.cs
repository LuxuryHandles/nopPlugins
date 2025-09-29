using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.Outcome.Services;

namespace Nop.Plugin.Widgets.Outcome.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration _)
        {
            services.AddScoped<IOutcomeService, OutcomeService>();
            services.AddScoped<IMentalHealthService, MentalHealthService>();
            services.AddScoped<IOutcomeStepsService, OutcomeStepsService>(); // existing implementation in your plugin
            services.AddScoped<IOutcomeDocExportService, OutcomeDocExportService>();
        }

        public void Configure(IApplicationBuilder _) { }
        public int Order => 3000;
    }
}
