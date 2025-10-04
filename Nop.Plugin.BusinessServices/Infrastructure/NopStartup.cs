using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.BusinessServices.Services;

namespace Nop.Plugin.BusinessServices.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBusinessCatalogService, BusinessCatalogService>();
        }
        public void Configure(IApplicationBuilder application) { }
        public int Order => 3000;
    }
}
