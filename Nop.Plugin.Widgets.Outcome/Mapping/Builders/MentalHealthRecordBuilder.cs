using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.Outcome.Domain;

namespace Nop.Plugin.Widgets.Outcome.Mapping.Builders
{
    public class MentalHealthRecordBuilder : NopEntityBuilder<MentalHealthRecord>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MentalHealthRecord.OutcomeRecordId)).AsInt32().NotNullable()
                .WithColumn(nameof(MentalHealthRecord.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(MentalHealthRecord.ProjectId)).AsInt32().NotNullable()

                // selection flags
                .WithColumn(nameof(MentalHealthRecord.M1_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M2_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M3_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M4_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M5_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M6_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M7_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M8_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M9_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M10_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M11_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M12_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M13_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M14_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(MentalHealthRecord.M15_Selected)).AsBoolean().NotNullable().WithDefaultValue(false)

                // inputs (match your previous working fields)
                .WithColumn(nameof(MentalHealthRecord.M1_Respondents)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M1_ReducedAnxietyCount)).AsByte().Nullable()

                .WithColumn(nameof(MentalHealthRecord.M2_Respondents)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M2_SurveyType)).AsByte().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M2_OneOff_Improved6mCount)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M2_PrePost_AvgImprovement)).AsDecimal(18, 2).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M3_Respondents)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M3_SurveyType)).AsByte().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M3_OneOff_Improved6mCount)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M3_PrePost_AvgImprovement)).AsDecimal(18, 2).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M4_ParticipantsTotal)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M4_FirstTimeParticipants)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M4_RepeatParticipants)).AsInt32().Nullable()

                .WithColumn(nameof(MentalHealthRecord.M5_Respondents)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M5_SurveyType)).AsByte().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M5_OneOff_Improved6mCount)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M5_PrePost_AvgImprovement)).AsDecimal(18, 2).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M6_Respondents)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M6_SurveyType)).AsByte().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M6_OneOff_Improved6mCount)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M6_PrePost_AvgImprovement)).AsDecimal(18, 2).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M7_GroupParticipantsTotal)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M7_GroupActivitiesPerWeek)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M7_ReferralNetworks)).AsString(int.MaxValue).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M8_Respondents)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M8_SurveyType)).AsByte().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M8_OneOff_Improved6mCount)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M8_PrePost_AvgImprovement)).AsDecimal(18, 2).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M9_Respondents)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M9_ManagedToHandleMentalHealthCount)).AsInt32().Nullable()

                .WithColumn(nameof(MentalHealthRecord.M10_FewerCrisisIncidentsCount)).AsInt32().Nullable()

                .WithColumn(nameof(MentalHealthRecord.M11_UrgentReferralsReductionPct)).AsDecimal(5, 2).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M12_RespondedCount)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M12_BetterAwarenessCount)).AsInt32().Nullable()

                .WithColumn(nameof(MentalHealthRecord.M13_LeaderObservations)).AsString(int.MaxValue).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M14_Respondents)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M14_SurveyType)).AsByte().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M14_OneOff_Improved6mCount)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M14_PrePost_AvgImprovement)).AsDecimal(18, 2).Nullable()

                .WithColumn(nameof(MentalHealthRecord.M15_ProgrammeCompletionsCount)).AsInt32().Nullable()
                .WithColumn(nameof(MentalHealthRecord.M15_NoRelapse6mPct)).AsDecimal(5, 2).Nullable();

            // Optional: unique index so you have at most one mental row per OutcomeRecord
            // (Create.Index().OnTable(nameof(MentalHealthRecord))...)
        }
    }
}
