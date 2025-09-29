namespace Nop.Plugin.Widgets.Outcome.Models
{
    // Matches the view & JS:
    // 0 = Pre, 1 = Post, 2 = One-off
    public enum SurveyType : byte
    {
        PreSurvey = 0,
        PostSurvey = 1,
        OneOffSurvey = 2
    }

    public enum ReducedAnxietyLevel : byte
    {
        //NotSet = 0,
        //Slight = 1,
        //Moderate = 2,
        //Significant = 3
        Yes = 0,
        ALot = 1,
        YesABit = 2
    }
}
