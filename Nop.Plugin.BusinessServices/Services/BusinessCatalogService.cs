using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Data;
using Nop.Data;
using Nop.Plugin.BusinessServices.Domain;

namespace Nop.Plugin.BusinessServices.Services
{
    public class BusinessCatalogService : IBusinessCatalogService
    {
        private readonly INopDataProvider _dataProvider;
        public BusinessCatalogService(INopDataProvider dataProvider) => _dataProvider = dataProvider;

        private static DataParameter P(string name, object value) => new DataParameter(name, value ?? DBNull.Value);

        // ---------------- BusinessType ----------------
        public async Task<BusinessType> GetBusinessTypeByIdAsync(int id)
        {
            var list = await _dataProvider.QueryAsync<BusinessType>(
                "SELECT TOP(1) * FROM [BusinessType] WHERE [Id]=@Id", P("@Id", id));
            return list.FirstOrDefault();
        }

        public async Task<IList<BusinessType>> GetAllBusinessTypesAsync(string name = null, bool? published = null)
        {
            var sb = new StringBuilder("SELECT * FROM [BusinessType] WHERE 1=1");
            var prms = new List<DataParameter>();
            if (!string.IsNullOrWhiteSpace(name))
            { sb.Append(" AND [Name] LIKE @Name"); prms.Add(P("@Name", $"%{name}%")); }
            if (published.HasValue)
            { sb.Append(" AND [Published]=@Pub"); prms.Add(P("@Pub", published.Value)); }
            sb.Append(" ORDER BY [Name]");
            return await _dataProvider.QueryAsync<BusinessType>(sb.ToString(), prms.ToArray());
        }

        public async Task InsertBusinessTypeAsync(BusinessType e)
        {
            e.CreatedOnUtc = DateTime.UtcNow;
            e.UpdatedOnUtc = e.CreatedOnUtc;
            var ids = await _dataProvider.QueryAsync<int>(
                @"INSERT INTO [BusinessType] ([Name],[Description],[Published],[CreatedOnUtc],[UpdatedOnUtc])
                  VALUES (@Name,@Desc,@Pub,@C,@U); SELECT CAST(SCOPE_IDENTITY() AS int);",
                P("@Name", e.Name), P("@Desc", (object)e.Description ?? DBNull.Value),
                P("@Pub", e.Published), P("@C", e.CreatedOnUtc), P("@U", e.UpdatedOnUtc));
            e.Id = ids.First();
        }

        public async Task UpdateBusinessTypeAsync(BusinessType e)
        {
            e.UpdatedOnUtc = DateTime.UtcNow;
            await _dataProvider.ExecuteNonQueryAsync(
                @"UPDATE [BusinessType] SET [Name]=@Name,[Description]=@Desc,[Published]=@Pub,[UpdatedOnUtc]=@U WHERE [Id]=@Id",
                P("@Name", e.Name), P("@Desc", (object)e.Description ?? DBNull.Value),
                P("@Pub", e.Published), P("@U", e.UpdatedOnUtc), P("@Id", e.Id));
        }

        public Task DeleteBusinessTypeAsync(BusinessType e)
            => _dataProvider.ExecuteNonQueryAsync("DELETE FROM [BusinessType] WHERE [Id]=@Id", P("@Id", e.Id));

        // ---------------- ServiceItem ----------------
        public async Task<ServiceItem> GetServiceItemByIdAsync(int id)
        {
            var list = await _dataProvider.QueryAsync<ServiceItem>(
                "SELECT TOP(1) * FROM [Services] WHERE [Id]=@Id", P("@Id", id));
            return list.FirstOrDefault();
        }

        public async Task<IList<ServiceItem>> GetAllServiceItemsAsync(string name = null, bool? published = null)
        {
            var sb = new StringBuilder("SELECT * FROM [Services] WHERE 1=1");
            var prms = new List<DataParameter>();
            if (!string.IsNullOrWhiteSpace(name))
            { sb.Append(" AND [Name] LIKE @Name"); prms.Add(P("@Name", $"%{name}%")); }
            if (published.HasValue)
            { sb.Append(" AND [Published]=@Pub"); prms.Add(P("@Pub", published.Value)); }
            sb.Append(" ORDER BY [Name]");
            return await _dataProvider.QueryAsync<ServiceItem>(sb.ToString(), prms.ToArray());
        }

        public async Task InsertServiceItemAsync(ServiceItem e)
        {
            e.CreatedOnUtc = DateTime.UtcNow;
            e.UpdatedOnUtc = e.CreatedOnUtc;
            var ids = await _dataProvider.QueryAsync<int>(
                @"INSERT INTO [Services] ([Name],[Description],[Published],[CreatedOnUtc],[UpdatedOnUtc])
                  VALUES (@Name,@Desc,@Pub,@C,@U); SELECT CAST(SCOPE_IDENTITY() AS int);",
                P("@Name", e.Name), P("@Desc", (object)e.Description ?? DBNull.Value),
                P("@Pub", e.Published), P("@C", e.CreatedOnUtc), P("@U", e.UpdatedOnUtc));
            e.Id = ids.First();
        }

        public async Task UpdateServiceItemAsync(ServiceItem e)
        {
            e.UpdatedOnUtc = DateTime.UtcNow;
            await _dataProvider.ExecuteNonQueryAsync(
                @"UPDATE [Services] SET [Name]=@Name,[Description]=@Desc,[Published]=@Pub,[UpdatedOnUtc]=@U WHERE [Id]=@Id",
                P("@Name", e.Name), P("@Desc", (object)e.Description ?? DBNull.Value),
                P("@Pub", e.Published), P("@U", e.UpdatedOnUtc), P("@Id", e.Id));
        }

        public Task DeleteServiceItemAsync(ServiceItem e)
            => _dataProvider.ExecuteNonQueryAsync("DELETE FROM [Services] WHERE [Id]=@Id", P("@Id", e.Id));

        // ---------------- Mapping ----------------
        public Task<IList<ServiceItem>> GetServicesForBusinessTypeAsync(int businessTypeId)
        {
            const string sql = @"
SELECT s.*
FROM [BusinessTypeServices] bts
JOIN [Services] s ON s.[Id] = bts.[ServiceItemId]
WHERE bts.[BusinessTypeId] = @B AND bts.[Published]=1 AND s.[Published]=1
ORDER BY s.[Name]";
            return _dataProvider.QueryAsync<ServiceItem>(sql, P("@B", businessTypeId));
        }

        public async Task MapServicesToBusinessTypeAsync(int businessTypeId, IEnumerable<int> serviceIds)
        {
            var ids = (serviceIds ?? Enumerable.Empty<int>()).Distinct().ToList();

            // Clear existing
            await _dataProvider.ExecuteNonQueryAsync(
                "DELETE FROM [BusinessTypeServices] WHERE [BusinessTypeId]=@B",
                P("@B", businessTypeId));

            if (ids.Count == 0)
                return;

            var now = DateTime.UtcNow;
            var values = string.Join(",", ids.Select((_, i) => $"(@B{i}, @S{i}, 1, @C{i}, @U{i})"));
            var prms = new List<DataParameter>();
            for (int i = 0; i < ids.Count; i++)
            {
                prms.Add(P($"@B{i}", businessTypeId));
                prms.Add(P($"@S{i}", ids[i]));
                prms.Add(P($"@C{i}", now));
                prms.Add(P($"@U{i}", now));
            }

            var sql = $@"INSERT INTO [BusinessTypeServices] ([BusinessTypeId],[ServiceItemId],[Published],[CreatedOnUtc],[UpdatedOnUtc]) VALUES {values};";
            await _dataProvider.ExecuteNonQueryAsync(sql, prms.ToArray());
        }

        // ---------------- Seeders ----------------
        public async Task<int> SeedDefaultBusinessTypesAsync()
        {
            var existing = await _dataProvider.QueryAsync<BusinessType>("SELECT [Name] FROM [BusinessType]");
            var have = existing.Select(x => (x.Name ?? "").Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var list = new[] { "Pet grooming", "Pet day care", "Kennels", "Vets" };
            var added = 0;
            foreach (var name in list)
            {
                if (have.Contains(name.Trim()))
                    continue;
                await InsertBusinessTypeAsync(new BusinessType { Name = name, Published = true, Description = null });
                added++;
            }
            return added;
        }

        public async Task<int> SeedDefaultServicesAsync()
        {
            var existing = await _dataProvider.QueryAsync<ServiceItem>("SELECT [Name] FROM [Services]");
            var have = existing.Select(x => (x.Name ?? "").Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var list = DefaultServices();
            var added = 0;
            foreach (var s in list)
            {
                if (have.Contains((s.Name ?? "").Trim()))
                    continue;
                await InsertServiceItemAsync(s);
                added++;
            }
            return added;
        }

        // NEW: full one-click seed with mapping
        public async Task<(int typesAdded, int servicesAdded, int mappingsAdded)> SeedAllWithMappingsAsync()
        {
            var typesAdded = await SeedDefaultBusinessTypesAsync();
            var servicesAdded = await SeedDefaultServicesAsync();

            // load ids
            var typeRows = await _dataProvider.QueryAsync<BusinessType>("SELECT [Id],[Name] FROM [BusinessType]");
            var typeByName = typeRows.ToDictionary(x => (x.Name ?? "").Trim(), x => x.Id, StringComparer.OrdinalIgnoreCase);

            var serviceRows = await _dataProvider.QueryAsync<ServiceItem>("SELECT [Id],[Name] FROM [Services]");
            var serviceByName = serviceRows.ToDictionary(x => (x.Name ?? "").Trim(), x => x.Id, StringComparer.OrdinalIgnoreCase);

            var maps = DefaultServiceMappings(); // (serviceName, typeNames[])

            var now = DateTime.UtcNow;
            var mappingsAdded = 0;

            foreach (var (serviceName, typeNames) in maps)
            {
                if (!serviceByName.TryGetValue(serviceName.Trim(), out var sid))
                    continue;

                foreach (var tname in typeNames)
                {
                    if (!typeByName.TryGetValue(tname.Trim(), out var tid))
                        continue;

                    // check existing
                    var exists = await _dataProvider.QueryAsync<int>(
                        "SELECT TOP(1) 1 FROM [BusinessTypeServices] WHERE [BusinessTypeId]=@B AND [ServiceItemId]=@S",
                        P("@B", tid), P("@S", sid));

                    if (!exists.Any())
                    {
                        await _dataProvider.ExecuteNonQueryAsync(
                            @"INSERT INTO [BusinessTypeServices]
                              ([BusinessTypeId],[ServiceItemId],[Published],[CreatedOnUtc],[UpdatedOnUtc])
                              VALUES (@B,@S,1,@C,@U)",
                            P("@B", tid), P("@S", sid), P("@C", now), P("@U", now));
                        mappingsAdded++;
                    }
                }
            }

            return (typesAdded, servicesAdded, mappingsAdded);
        }

        // Short but meaningful starter set (map per type)
        private static IList<ServiceItem> DefaultServices() => new List<ServiceItem>
        {
            // Grooming
            new ServiceItem{ Name="Bath & blow-dry", Published=true },
            new ServiceItem{ Name="Wash & dry (quick bath)", Published=true },
            new ServiceItem{ Name="Full groom/clip (breed style)", Published=true },
            new ServiceItem{ Name="Puppy/kitten intro groom", Published=true },
            new ServiceItem{ Name="De-shedding / coat control", Published=true },
            new ServiceItem{ Name="De-matting / coat tidy", Published=true },
            new ServiceItem{ Name="Sanitary trim", Published=true },
            new ServiceItem{ Name="Face tidy / eye area trim", Published=true },
            new ServiceItem{ Name="Paw pad trim / feather trim", Published=true },
            new ServiceItem{ Name="Nail clipping", Published=true },
            new ServiceItem{ Name="Ear cleaning", Published=true },
            new ServiceItem{ Name="Teeth brushing (cosmetic)", Published=true },

            // Day care
            new ServiceItem{ Name="Half-day supervised play", Published=true },
            new ServiceItem{ Name="Full-day supervised play", Published=true },
            new ServiceItem{ Name="Enrichment sessions", Published=true },
            new ServiceItem{ Name="Lead walks", Published=true },

            // Kennel
            new ServiceItem{ Name="Standard overnight boarding", Published=true },
            new ServiceItem{ Name="Luxury suite boarding", Published=true },
            new ServiceItem{ Name="Individual play/walks (boarding)", Published=true },
            new ServiceItem{ Name="Bath & tidy before checkout", Published=true },

            // Vet
            new ServiceItem{ Name="Wellness exam", Published=true },
            new ServiceItem{ Name="Vaccinations", Published=true },
            new ServiceItem{ Name="Microchipping", Published=true },
            new ServiceItem{ Name="Dental scale & polish", Published=true }
        };

        // Map services to the appropriate types
        // Types: "Pet grooming", "Day care", "Kennel", "Vet"
        private static List<(string ServiceName, string[] TypeNames)> DefaultServiceMappings()
        {
            var list = new List<(string, string[])>();
            void Add(string name, bool g = false, bool d = false, bool k = false, bool v = false)
            {
                var types = new List<string>();
                if (g)
                    types.Add("Pet grooming");
                if (d)
                    types.Add("Pet day care");
                if (k)
                    types.Add("Kennels");
                if (v)
                    types.Add("Vets");
                list.Add((name, types.ToArray()));
            }

            // Grooming-centric
            Add("Bath & blow-dry", g: true);
            Add("Wash & dry (quick bath)", g: true, d: true, k: true);
            Add("Full groom/clip (breed style)", g: true);
            Add("Puppy/kitten intro groom", g: true);
            Add("De-shedding / coat control", g: true);
            Add("De-matting / coat tidy", g: true);
            Add("Sanitary trim", g: true);
            Add("Face tidy / eye area trim", g: true);
            Add("Paw pad trim / feather trim", g: true);

            // Cross-category basics
            Add("Nail clipping", g: true, d: true, k: true, v: true);
            Add("Ear cleaning", g: true, v: true);
            Add("Teeth brushing (cosmetic)", g: true, d: true, k: true);

            // Day care
            Add("Half-day supervised play", d: true);
            Add("Full-day supervised play", d: true);
            Add("Enrichment sessions", d: true);
            Add("Lead walks", d: true);

            // Kennel/boarding
            Add("Standard overnight boarding", k: true);
            Add("Luxury suite boarding", k: true);
            Add("Individual play/walks (boarding)", k: true);
            Add("Bath & tidy before checkout", k: true);

            // Vet
            Add("Wellness exam", v: true);
            Add("Vaccinations", v: true);
            Add("Microchipping", v: true);
            Add("Dental scale & polish", v: true);

            return list;
        }

        // Bulk read of all mappings (for grid matrix)
        public async Task<IList<(int BusinessTypeId, int ServiceItemId)>> GetAllMappingsAsync()
        {
            var rows = await _dataProvider.QueryAsync<MapRow>(
                "SELECT [BusinessTypeId],[ServiceItemId] FROM [BusinessTypeServices]");

            return rows.Select(r => (r.BusinessTypeId, r.ServiceItemId)).ToList();
        }

        private sealed class MapRow
        {
            public int BusinessTypeId { get; set; }
            public int ServiceItemId { get; set; }
        }
    }
}
