using System.Linq;
using LinqToDB.Data;      // DataParameter
using Nop.Data;

namespace Nop.Plugin.Widgets.Outcome.Services
{
    public class OutcomeStepsService : IOutcomeStepsService
    {
        private readonly INopDataProvider _dataProvider;
        public OutcomeStepsService(INopDataProvider dataProvider) => _dataProvider = dataProvider;

        public async Task<int?> GetOutcomeIdAsync(int customerId, int projectId)
        {
            const string sql = @"
                SELECT TOP (1) [OutcomeId]
                FROM [Steps]
                WHERE [CustomerId] = @CustomerId AND [ProjectId] = @ProjectId
                ORDER BY [Id] DESC;";

            var rows = await _dataProvider.QueryAsync<int?>(
                sql,
                new DataParameter("CustomerId", customerId),
                new DataParameter("ProjectId", projectId));

            return rows.FirstOrDefault();
        }

        public Task<int> UpdateOutcomeLinkAndStep3OnlyAsync(int customerId, int projectId, int outcomeId)
        {
            const string sql = @"
                UPDATE [Steps]
                    SET [Step3] = 1,
                        [OutcomeId] = @OutcomeId
                WHERE [CustomerId] = @CustomerId
                  AND [ProjectId] = @ProjectId;";

            return _dataProvider.ExecuteNonQueryAsync(
                sql,
                new DataParameter("CustomerId", customerId),
                new DataParameter("ProjectId", projectId),
                new DataParameter("OutcomeId", outcomeId));
        }
    }
}
