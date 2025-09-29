namespace Nop.Plugin.Widgets.Outcome.Services
{
    public interface IOutcomeStepsService
    {
        /// <summary>Returns OutcomeId from Steps for this (customerId, projectId), or null if none.</summary>
        Task<int?> GetOutcomeIdAsync(int customerId, int projectId);

        /// <summary>
        /// Updates the existing Steps row (no insert) to set Step3 = 1 and OutcomeId.
        /// Returns affected rows (0 if no Steps row found).
        /// </summary>
        Task<int> UpdateOutcomeLinkAndStep3OnlyAsync(int customerId, int projectId, int outcomeId);
    }
}
