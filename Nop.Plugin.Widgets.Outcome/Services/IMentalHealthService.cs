using System.Threading.Tasks;
using Nop.Plugin.Widgets.Outcome.Domain;

namespace Nop.Plugin.Widgets.Outcome.Services
{
    public interface IMentalHealthService
    {
        Task<MentalHealthRecord?> GetByOutcomeIdAsync(int outcomeRecordId, int customerId);
        Task InsertAsync(MentalHealthRecord entity);
        Task UpdateAsync(MentalHealthRecord entity);
    }
}
