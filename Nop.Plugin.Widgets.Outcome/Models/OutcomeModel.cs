using System;
using System.ComponentModel;

namespace Nop.Plugin.Widgets.Outcome.Models
{
    public class OutcomeModel
    {
        // -----------------------------
        // Identity / state
        // -----------------------------
        public int Id { get; set; }           // OutcomeRecord Id
        public int ProjectId { get; set; }
        public bool Existing { get; set; }

        // -----------------------------
        // Top categories (Level 1)
        // -----------------------------
        public bool Cat_MentalHealth { get; set; }
        public bool Cat_PhysicalHealth { get; set; }
        public bool Cat_Employment { get; set; }
        public bool Cat_SocialConnection { get; set; }
        public bool Cat_Culture { get; set; }
        public bool Cat_Housing { get; set; }
        public bool Cat_Religious { get; set; }
        public bool Cat_Environment { get; set; }

        // -----------------------------
        // Nested models
        // -----------------------------
        public MentalHealthModel Mental { get; set; } = new();

        // -----------------------------
        // View constants (titles used by Index.cshtml)
        // -----------------------------
        #region Mental health & wellbeing titles (used by Index.cshtml)
        public const string M1_Title = "People feel less anxious or worried (M1)";
        public const string M2_Title = "People feel less alone and more connected (M2)";
        public const string M3_Title = "People are sleeping and eating better (M3)";
        public const string M4_Title = "People are getting involved in activities again (M4)";
        public const string M5_Title = "People feel more confident and hopeful (M5)";
        public const string M6_Title = "People feel more positive, bounce back more easily, and/or have a clearer sense of direction (M6)";
        public const string M7_Title = "More people are joining group sessions or support networks (M7)";
        public const string M8_Title = "People say they feel better and are coping better day to day (M8)";
        public const string M9_Title = "People feel more able to look after their mental health themselves (M9)";
        public const string M10_Title = "Reduced use of crisis or emergency health services (M10)";
        public const string M11_Title = "Fewer people are needing urgent mental health support (M11)";
        public const string M12_Title = "People better understand mental health and what support is available (M12)";
        public const string M13_Title = "People feel more comfortable opening up and talking about how they feel (M13)";
        public const string M14_Title = "People say they feel happier overall (M14)";
        public const string M15_Title = "People have reduced harmful behaviours e.g. substance misuse, overeating, addictions (M15)";
        #endregion

        // ============================================================
        // Wrapper properties (keep Razor unchanged, write through to Mental.*)
        // ============================================================

        // -----------------------------
        // M1
        // --------
        // ---------------------
        public bool M1_Selected
        {
            get => Mental?.M1_Selected ?? false;
            set { (Mental ??= new()).M1_Selected = value; }
        }

        [DisplayName("How many people responded?")]
        public int? M1_Respondents
        {
            get => Mental?.M1_Respondents;
            set { (Mental ??= new()).M1_Respondents = value; }
        }
        [DisplayName("Number who reported reduced anxiety or worry")]
        public ReducedAnxietyLevel? M1_ReducedAnxietyCount
        {
            get => Mental?.M1_ReducedAnxietyCount;
            set { (Mental ??= new()).M1_ReducedAnxietyCount = value; }
        }

        // -----------------------------
        // M2 (survey-type)
        // -----------------------------
        public bool M2_Selected
        {
            get => Mental?.M2_Selected ?? false;
            set { (Mental ??= new()).M2_Selected = value; }
        }
        [DisplayName("How many people responded?")]
        public int? M2_Respondents
        {
            get => Mental?.M2_Respondents;
            set { (Mental ??= new()).M2_Respondents = value; }
        }
        [DisplayName("Did you do a pre and post survey or a one-off survey?")]
        public SurveyType? M2_SurveyType
        {
            get => Mental?.M2_SurveyType;
            set { (Mental ??= new()).M2_SurveyType = value; }
        }
        [DisplayName("For one-off surveys: How many reported improvements in the last 6 months?")]
        public int? M2_OneOff_Improved6mCount
        {
            get => Mental?.M2_OneOff_Improved6mCount;
            set { (Mental ??= new()).M2_OneOff_Improved6mCount = value; }
        }
        [DisplayName("For pre and post surveys: Average improvement value")]
        public decimal? M2_PrePost_AvgImprovement
        {
            get => Mental?.M2_PrePost_AvgImprovement;
            set { (Mental ??= new()).M2_PrePost_AvgImprovement = value; }
        }

        // -----------------------------
        // M3 (survey-type)
        // -----------------------------
        public bool M3_Selected
        {
            get => Mental?.M3_Selected ?? false;
            set { (Mental ??= new()).M3_Selected = value; }
        }
        public int? M3_Respondents
        {
            get => Mental?.M3_Respondents;
            set { (Mental ??= new()).M3_Respondents = value; }
        }
        public SurveyType? M3_SurveyType
        {
            get => Mental?.M3_SurveyType;
            set { (Mental ??= new()).M3_SurveyType = value; }
        }
        public int? M3_OneOff_Improved6mCount
        {
            get => Mental?.M3_OneOff_Improved6mCount;
            set { (Mental ??= new()).M3_OneOff_Improved6mCount = value; }
        }
        public decimal? M3_PrePost_AvgImprovement
        {
            get => Mental?.M3_PrePost_AvgImprovement;
            set { (Mental ??= new()).M3_PrePost_AvgImprovement = value; }
        }

        // -----------------------------
        // M4
        // -----------------------------
        public bool M4_Selected
        {
            get => Mental?.M4_Selected ?? false;
            set { (Mental ??= new()).M4_Selected = value; }
        }
        public int? M4_ParticipantsTotal
        {
            get => Mental?.M4_ParticipantsTotal;
            set { (Mental ??= new()).M4_ParticipantsTotal = value; }
        }
        public int? M4_FirstTimeParticipants
        {
            get => Mental?.M4_FirstTimeParticipants;
            set { (Mental ??= new()).M4_FirstTimeParticipants = value; }
        }
        public int? M4_RepeatParticipants
        {
            get => Mental?.M4_RepeatParticipants;
            set { (Mental ??= new()).M4_RepeatParticipants = value; }
        }

        // -----------------------------
        // M5 (survey-type)
        // -----------------------------
        public bool M5_Selected
        {
            get => Mental?.M5_Selected ?? false;
            set { (Mental ??= new()).M5_Selected = value; }
        }
        public int? M5_Respondents
        {
            get => Mental?.M5_Respondents;
            set { (Mental ??= new()).M5_Respondents = value; }
        }
        public SurveyType? M5_SurveyType
        {
            get => Mental?.M5_SurveyType;
            set { (Mental ??= new()).M5_SurveyType = value; }
        }
        public int? M5_OneOff_Improved6mCount
        {
            get => Mental?.M5_OneOff_Improved6mCount;
            set { (Mental ??= new()).M5_OneOff_Improved6mCount = value; }
        }
        public decimal? M5_PrePost_AvgImprovement
        {
            get => Mental?.M5_PrePost_AvgImprovement;
            set { (Mental ??= new()).M5_PrePost_AvgImprovement = value; }
        }

        // -----------------------------
        // M6 (survey-type)
        // -----------------------------
        public bool M6_Selected
        {
            get => Mental?.M6_Selected ?? false;
            set { (Mental ??= new()).M6_Selected = value; }
        }
        public int? M6_Respondents
        {
            get => Mental?.M6_Respondents;
            set { (Mental ??= new()).M6_Respondents = value; }
        }
        public SurveyType? M6_SurveyType
        {
            get => Mental?.M6_SurveyType;
            set { (Mental ??= new()).M6_SurveyType = value; }
        }
        public int? M6_OneOff_Improved6mCount
        {
            get => Mental?.M6_OneOff_Improved6mCount;
            set { (Mental ??= new()).M6_OneOff_Improved6mCount = value; }
        }
        public decimal? M6_PrePost_AvgImprovement
        {
            get => Mental?.M6_PrePost_AvgImprovement;
            set { (Mental ??= new()).M6_PrePost_AvgImprovement = value; }
        }

        // -----------------------------
        // M7
        // -----------------------------
        public bool M7_Selected
        {
            get => Mental?.M7_Selected ?? false;
            set { (Mental ??= new()).M7_Selected = value; }
        }
        public int? M7_GroupParticipantsTotal
        {
            get => Mental?.M7_GroupParticipantsTotal;
            set { (Mental ??= new()).M7_GroupParticipantsTotal = value; }
        }
        public int? M7_GroupActivitiesPerWeek
        {
            get => Mental?.M7_GroupActivitiesPerWeek;
            set { (Mental ??= new()).M7_GroupActivitiesPerWeek = value; }
        }
        public string M7_ReferralNetworks
        {
            get => Mental?.M7_ReferralNetworks;
            set { (Mental ??= new()).M7_ReferralNetworks = value; }
        }

        // -----------------------------
        // M8 (survey-type)
        // -----------------------------
        public bool M8_Selected
        {
            get => Mental?.M8_Selected ?? false;
            set { (Mental ??= new()).M8_Selected = value; }
        }
        public int? M8_Respondents
        {
            get => Mental?.M8_Respondents;
            set { (Mental ??= new()).M8_Respondents = value; }
        }
        public SurveyType? M8_SurveyType
        {
            get => Mental?.M8_SurveyType;
            set { (Mental ??= new()).M8_SurveyType = value; }
        }
        public int? M8_OneOff_Improved6mCount
        {
            get => Mental?.M8_OneOff_Improved6mCount;
            set { (Mental ??= new()).M8_OneOff_Improved6mCount = value; }
        }
        public decimal? M8_PrePost_AvgImprovement
        {
            get => Mental?.M8_PrePost_AvgImprovement;
            set { (Mental ??= new()).M8_PrePost_AvgImprovement = value; }
        }

        // -----------------------------
        // M9
        // -----------------------------
        public bool M9_Selected
        {
            get => Mental?.M9_Selected ?? false;
            set { (Mental ??= new()).M9_Selected = value; }
        }
        public int? M9_Respondents
        {
            get => Mental?.M9_Respondents;
            set { (Mental ??= new()).M9_Respondents = value; }
        }
        public int? M9_ManagedToHandleMentalHealthCount
        {
            get => Mental?.M9_ManagedToHandleMentalHealthCount;
            set { (Mental ??= new()).M9_ManagedToHandleMentalHealthCount = value; }
        }

        // -----------------------------
        // M10
        // -----------------------------
        public bool M10_Selected
        {
            get => Mental?.M10_Selected ?? false;
            set { (Mental ??= new()).M10_Selected = value; }
        }
        public int? M10_FewerCrisisIncidentsCount
        {
            get => Mental?.M10_FewerCrisisIncidentsCount;
            set { (Mental ??= new()).M10_FewerCrisisIncidentsCount = value; }
        }

        // -----------------------------
        // M11
        // -----------------------------
        public bool M11_Selected
        {
            get => Mental?.M11_Selected ?? false;
            set { (Mental ??= new()).M11_Selected = value; }
        }
        public decimal? M11_UrgentReferralsReductionPct
        {
            get => Mental?.M11_UrgentReferralsReductionPct;
            set { (Mental ??= new()).M11_UrgentReferralsReductionPct = value; }
        }

        // -----------------------------
        // M12
        // -----------------------------
        public bool M12_Selected
        {
            get => Mental?.M12_Selected ?? false;
            set { (Mental ??= new()).M12_Selected = value; }
        }
        public int? M12_RespondedCount
        {
            get => Mental?.M12_RespondedCount;
            set { (Mental ??= new()).M12_RespondedCount = value; }
        }
        public int? M12_BetterAwarenessCount
        {
            get => Mental?.M12_BetterAwarenessCount;
            set { (Mental ??= new()).M12_BetterAwarenessCount = value; }
        }

        // -----------------------------
        // M13
        // -----------------------------
        public bool M13_Selected
        {
            get => Mental?.M13_Selected ?? false;
            set { (Mental ??= new()).M13_Selected = value; }
        }
        public string M13_LeaderObservations
        {
            get => Mental?.M13_LeaderObservations;
            set { (Mental ??= new()).M13_LeaderObservations = value; }
        }

        // -----------------------------
        // M14 (survey-type)
        // -----------------------------
        public bool M14_Selected
        {
            get => Mental?.M14_Selected ?? false;
            set { (Mental ??= new()).M14_Selected = value; }
        }
        public int? M14_Respondents
        {
            get => Mental?.M14_Respondents;
            set { (Mental ??= new()).M14_Respondents = value; }
        }
        public SurveyType? M14_SurveyType
        {
            get => Mental?.M14_SurveyType;
            set { (Mental ??= new()).M14_SurveyType = value; }
        }
        public int? M14_OneOff_Improved6mCount
        {
            get => Mental?.M14_OneOff_Improved6mCount;
            set { (Mental ??= new()).M14_OneOff_Improved6mCount = value; }
        }
        public decimal? M14_PrePost_AvgImprovement
        {
            get => Mental?.M14_PrePost_AvgImprovement;
            set { (Mental ??= new()).M14_PrePost_AvgImprovement = value; }
        }

        // -----------------------------
        // M15
        // -----------------------------
        public bool M15_Selected
        {
            get => Mental?.M15_Selected ?? false;
            set { (Mental ??= new()).M15_Selected = value; }
        }
        public int? M15_ProgrammeCompletionsCount
        {
            get => Mental?.M15_ProgrammeCompletionsCount;
            set { (Mental ??= new()).M15_ProgrammeCompletionsCount = value; }
        }
        public decimal? M15_NoRelapse6mPct
        {
            get => Mental?.M15_NoRelapse6mPct;
            set { (Mental ??= new()).M15_NoRelapse6mPct = value; }
        }

        // -----------------------------
        // UI flash messages
        // -----------------------------
        public string? InfoMessage { get; set; }
        public string? InfoMessageType { get; set; }
    }
}
