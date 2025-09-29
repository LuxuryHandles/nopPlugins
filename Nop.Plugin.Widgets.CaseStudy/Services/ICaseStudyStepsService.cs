using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CaseStudy.Services
{
    public interface ICaseStudyStepsService
    {
        /// <summary>
        /// Ensure a Steps row exists for (CustomerId, ProjectId). Set Step4=1 and CaseStudyID={id}.
        /// </summary>
        Task UpsertCaseStudyLinkAndStep4Async(int customerId, int projectId, int caseStudyId);
        Task<int?> GetCaseStudyIdForAsync(int customerId, int projectId);
    }
}
