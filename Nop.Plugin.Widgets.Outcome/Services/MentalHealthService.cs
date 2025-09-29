using System.Linq;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Widgets.Outcome.Domain;

namespace Nop.Plugin.Widgets.Outcome.Services
{
    public class MentalHealthService : IMentalHealthService
    {
        private readonly IRepository<MentalHealthRecord> _repo;
        public MentalHealthService(IRepository<MentalHealthRecord> repo) => _repo = repo;

        public Task<MentalHealthRecord?> GetByOutcomeIdAsync(int outcomeRecordId, int customerId)
        {
            var result = _repo.Table.FirstOrDefault(x => x.OutcomeRecordId == outcomeRecordId && x.CustomerId == customerId);
            return Task.FromResult(result);
        }

        public Task InsertAsync(MentalHealthRecord entity) => _repo.InsertAsync(entity);
        public Task UpdateAsync(MentalHealthRecord entity) => _repo.UpdateAsync(entity);
    }
}
