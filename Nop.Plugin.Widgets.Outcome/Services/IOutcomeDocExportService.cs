using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Outcome.Services
{
    public interface IOutcomeDocExportService
    {
        Task<byte[]> BuildOutcomeDocAsync(int projectId, int outcomeId);
    }
}
