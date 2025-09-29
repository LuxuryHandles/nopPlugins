using Nop.Core;
using Nop.Plugin.Widgets.Outcome.Models;
using StackExchange.Redis;

namespace Nop.Plugin.Widgets.Outcome.Domain
{
    /// <summary>
    /// Stores all sub-categories (M1..M15) selections + inputs for "Mental health & wellbeing".
    /// One row per OutcomeRecord (per project/customer).
    /// </summary>
    public class MentalHealthRecord : BaseEntity
    {
        public int OutcomeRecordId { get; set; }
        public int CustomerId { get; set; }
        public int ProjectId { get; set; }

        // --- Selection flags (persist even if inputs are empty)
        public bool M1_Selected { get; set; }
        public bool M2_Selected { get; set; }
        public bool M3_Selected { get; set; }
        public bool M4_Selected { get; set; }
        public bool M5_Selected { get; set; }
        public bool M6_Selected { get; set; }
        public bool M7_Selected { get; set; }
        public bool M8_Selected { get; set; }
        public bool M9_Selected { get; set; }
        public bool M10_Selected { get; set; }
        public bool M11_Selected { get; set; }
        public bool M12_Selected { get; set; }
        public bool M13_Selected { get; set; }
        public bool M14_Selected { get; set; }
        public bool M15_Selected { get; set; }

        // --- Inputs (B–F from your sheet; mirrors your previous working fields)
        public int? M1_Respondents { get; set; }
        public ReducedAnxietyLevel? M1_ReducedAnxietyCount { get; set; }

        public int? M2_Respondents { get; set; }
        public SurveyType? M2_SurveyType { get; set; }
        public int? M2_OneOff_Improved6mCount { get; set; }
        public decimal? M2_PrePost_AvgImprovement { get; set; }

        public int? M3_Respondents { get; set; }
        public SurveyType? M3_SurveyType { get; set; }
        public int? M3_OneOff_Improved6mCount { get; set; }
        public decimal? M3_PrePost_AvgImprovement { get; set; }

        public int? M4_ParticipantsTotal { get; set; }
        public int? M4_FirstTimeParticipants { get; set; }
        public int? M4_RepeatParticipants { get; set; }

        public int? M5_Respondents { get; set; }
        public SurveyType? M5_SurveyType { get; set; }
        public int? M5_OneOff_Improved6mCount { get; set; }
        public decimal? M5_PrePost_AvgImprovement { get; set; }

        public int? M6_Respondents { get; set; }
        public SurveyType? M6_SurveyType { get; set; }
        public int? M6_OneOff_Improved6mCount { get; set; }
        public decimal? M6_PrePost_AvgImprovement { get; set; }

        public int? M7_GroupParticipantsTotal { get; set; }
        public int? M7_GroupActivitiesPerWeek { get; set; }
        public string? M7_ReferralNetworks { get; set; }

        public int? M8_Respondents { get; set; }
        public SurveyType? M8_SurveyType { get; set; }
        public int? M8_OneOff_Improved6mCount { get; set; }
        public decimal? M8_PrePost_AvgImprovement { get; set; }

        public int? M9_Respondents { get; set; }
        public int? M9_ManagedToHandleMentalHealthCount { get; set; }

        public int? M10_FewerCrisisIncidentsCount { get; set; }

        public decimal? M11_UrgentReferralsReductionPct { get; set; }

        public int? M12_RespondedCount { get; set; }
        public int? M12_BetterAwarenessCount { get; set; }

        public string? M13_LeaderObservations { get; set; }

        public int? M14_Respondents { get; set; }
        public SurveyType? M14_SurveyType { get; set; }
        public int? M14_OneOff_Improved6mCount { get; set; }
        public decimal? M14_PrePost_AvgImprovement { get; set; }

        public int? M15_ProgrammeCompletionsCount { get; set; }
        public decimal? M15_NoRelapse6mPct { get; set; }
    }
}
