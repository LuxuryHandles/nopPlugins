using System.Linq;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Widgets.CaseStudy.Domain;

namespace Nop.Plugin.Widgets.CaseStudy.Services
{
    public class CaseStudyService : ICaseStudyService
    {
        private readonly IRepository<CaseStudyRecord> _repo;
        public CaseStudyService(IRepository<CaseStudyRecord> repo) => _repo = repo;

        public Task<CaseStudyRecord?> GetByIdForCustomerAsync(int id, int customerId)
            => _repo.Table.FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId && !x.Deleted);

        public Task<CaseStudyRecord?> GetByProjectForCustomerAsync(int projectId, int customerId)
            => _repo.Table.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.CustomerId == customerId && !x.Deleted);

        public Task InsertAsync(CaseStudyRecord entity) => _repo.InsertAsync(entity);
        public Task UpdateAsync(CaseStudyRecord entity) => _repo.UpdateAsync(entity);

        public Task SoftDeleteAsync(CaseStudyRecord entity)
        {
            entity.Deleted = true;
            entity.Published = false;
            return _repo.UpdateAsync(entity);
        }
    }
}
