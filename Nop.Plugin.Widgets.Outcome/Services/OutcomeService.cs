using System.Linq;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Widgets.Outcome.Domain;

namespace Nop.Plugin.Widgets.Outcome.Services
{
    public class OutcomeService : IOutcomeService
    {
        private readonly IRepository<OutcomeRecord> _repo;
        public OutcomeService(IRepository<OutcomeRecord> repo) => _repo = repo;

        public Task<OutcomeRecord?> GetByIdForCustomerAsync(int outcomeId, int customerId)
        {
            var result = _repo.Table.FirstOrDefault(x => x.Id == outcomeId && x.CustomerId == customerId);
            return Task.FromResult(result);
        }

        public Task<OutcomeRecord?> GetByProjectForCustomerAsync(int projectId, int customerId)
        {
            var result = _repo.Table.FirstOrDefault(x => x.ProjectId == projectId && x.CustomerId == customerId);
            return Task.FromResult(result);
        }

        public Task InsertAsync(OutcomeRecord entity) => _repo.InsertAsync(entity);
        public Task UpdateAsync(OutcomeRecord entity) => _repo.UpdateAsync(entity);
    }
}
