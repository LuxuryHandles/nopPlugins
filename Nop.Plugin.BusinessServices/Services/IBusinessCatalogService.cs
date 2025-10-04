using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.BusinessServices.Domain;

namespace Nop.Plugin.BusinessServices.Services
{
    public interface IBusinessCatalogService
    {
        // BusinessType
        Task<BusinessType> GetBusinessTypeByIdAsync(int id);
        Task<IList<BusinessType>> GetAllBusinessTypesAsync(string name = null, bool? published = null);
        Task InsertBusinessTypeAsync(BusinessType entity);
        Task UpdateBusinessTypeAsync(BusinessType entity);
        Task DeleteBusinessTypeAsync(BusinessType entity);

        // ServiceItem
        Task<ServiceItem> GetServiceItemByIdAsync(int id);
        Task<IList<ServiceItem>> GetAllServiceItemsAsync(string name = null, bool? published = null);
        Task InsertServiceItemAsync(ServiceItem entity);
        Task UpdateServiceItemAsync(ServiceItem entity);
        Task DeleteServiceItemAsync(ServiceItem entity);

        // Mapping
        Task<IList<ServiceItem>> GetServicesForBusinessTypeAsync(int businessTypeId);
        Task MapServicesToBusinessTypeAsync(int businessTypeId, IEnumerable<int> serviceIds);

        // Seeders
        Task<int> SeedDefaultBusinessTypesAsync();
        Task<int> SeedDefaultServicesAsync();

        // NEW: all-in-one seeder
        Task<(int typesAdded, int servicesAdded, int mappingsAdded)> SeedAllWithMappingsAsync();

        // Mapping (bulk read)
        Task<IList<(int BusinessTypeId, int ServiceItemId)>> GetAllMappingsAsync();

    }
}
