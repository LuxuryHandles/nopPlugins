using System.Threading.Tasks;
using LinqToDB.Data;
using Nop.Data;

namespace Nop.Plugin.Widgets.CaseStudy.Services
{
    public class CaseStudyStepsService : ICaseStudyStepsService
    {
        private readonly INopDataProvider _dataProvider;
        public CaseStudyStepsService(INopDataProvider dataProvider) => _dataProvider = dataProvider;

        public Task UpsertCaseStudyLinkAndStep4Async(int customerId, int projectId, int caseStudyId)
        {
            var sql = @"
                        IF EXISTS (SELECT 1 FROM [Steps] WHERE [CustomerId] = @CustomerId AND [ProjectId] = @ProjectId)
                        BEGIN
                            UPDATE [Steps]
                               SET [CaseStudyID] = @CaseStudyID,
                                   [Step4] = 1
                             WHERE [CustomerId] = @CustomerId AND [ProjectId] = @ProjectId;
                        END
                        ELSE
                        BEGIN
                            INSERT INTO [Steps] ([CustomerId], [ProjectId], [CaseStudyID], [Step4])
                            VALUES (@CustomerId, @ProjectId, @CaseStudyID, 1);
                        END";

            var pCustomerId = new DataParameter("CustomerId", customerId);
            var pProjectId = new DataParameter("ProjectId", projectId);
            var pCaseStudyId = new DataParameter("CaseStudyID", caseStudyId);

            return _dataProvider.ExecuteNonQueryAsync(sql, pCustomerId, pProjectId, pCaseStudyId);
        }

        public async Task<int?> GetCaseStudyIdForAsync(int customerId, int projectId)
        {
            var sql = @"
                        SELECT TOP (1) [CaseStudyID]
                        FROM [Steps]
                        WHERE [CustomerId] = @CustomerId AND [ProjectId] = @ProjectId";

            var pCustomerId = new DataParameter("CustomerId", customerId);
            var pProjectId = new DataParameter("ProjectId", projectId);

            // INopDataProvider pattern you already use elsewhere (ExecuteNonQueryAsync) — 
            // here we use QueryAsync<T> to fetch a scalar row.
            var rows = await _dataProvider.QueryAsync<int?>(sql, pCustomerId, pProjectId);
            return rows?.FirstOrDefault();
        }
    }
}
