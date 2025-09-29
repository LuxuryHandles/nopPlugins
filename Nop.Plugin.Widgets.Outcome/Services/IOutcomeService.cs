using Nop.Plugin.Widgets.Outcome.Domain;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Outcome.Services
{
    public interface IOutcomeService
    {
        Task<OutcomeRecord?> GetByIdForCustomerAsync(int id, int customerId);
        Task<OutcomeRecord?> GetByProjectForCustomerAsync(int projectId, int customerId);
        Task InsertAsync(OutcomeRecord entity);
        Task UpdateAsync(OutcomeRecord entity);
    }
}
