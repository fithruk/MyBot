namespace MyBot.Classes;

public static class SurveyKeys
{
    
    private static readonly Dictionary<string, string?> TransliteratedKeys = new Dictionary<string, string?>() {
        { "чоловік", "male" },
        { "11-14", "11-14" },
        { "15-18", "15-18" },
        { "19-28", "19-28" },
        { "29-39", "29-39" },
        { "40 +", "40 +" },
        { "жінка", "female" },
        { "15-19", "15-19" },
        { "20-29", "20-29" },
        { "30-35", "30-35" },
        { "36-44", "36-44" },
        { "44 +", "44 +" },
        {"Я поки що не змогла.", "WK_test_false"},
        {"Так. Я це змогла зробити.", "WK_test_true"},
        {"Я поки що не зміг це зробити.", "WK_test_false"},
        {"Я зміг це зробити.", "WK_test_true"},
        {"НІ. Довжини пальців забракло.", "Genetic_test_false"},
        {"ТАК. Можу.", "Genetic_test_true"},
    };
    
    private static readonly Dictionary<string, string?> TrainerConversationKeys = new Dictionary<string, string?>()
    {
        {"name", "name"},
        {"age", "age"},
        {"height", "height"},
        {"weight", "weight"},
        {"weightPurpose", "weightPurpose"},
        {"target", "target"},
        {"workoutBackground", "workoutBackground"},
        {"workoutZone", "workoutZone"},
        {"howOften", "howOften"},
        {"motivation", "motivation"},
        {"allergyException", "allergyException"},
        {"foodSchedule", "foodSchedule"},
        {"isVegan", "isVegan"},
        {"isCalculateCal", "isCalculateCal"},
        {"conversationSubject", "conversationSubject"},
        {"additionalQuestion", "additionalQuestion"},
    };
    
    public static Dictionary<string, string> GetWkProgramQuestionsKeys()
    {
        return TransliteratedKeys!;
    }
}