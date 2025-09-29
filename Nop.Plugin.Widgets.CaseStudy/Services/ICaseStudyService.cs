using System.Threading.Tasks;
using Nop.Plugin.Widgets.CaseStudy.Domain;

namespace Nop.Plugin.Widgets.CaseStudy.Services
{
    public interface ICaseStudyService
    {
        Task<CaseStudyRecord?> GetByIdForCustomerAsync(int id, int customerId);
        Task<CaseStudyRecord?> GetByProjectForCustomerAsync(int projectId, int customerId);
        Task InsertAsync(CaseStudyRecord entity);
        Task UpdateAsync(CaseStudyRecord entity);
        Task SoftDeleteAsync(CaseStudyRecord entity);
    }
}
