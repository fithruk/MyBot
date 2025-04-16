using System.Text.Json;
using MyBot.Classes;

namespace MyBot.MyBotUsersDBJson;

public class SurveyQuestionsDB
{ 
    private readonly string _pathFile = Path.Combine(AppContext.BaseDirectory, "../../../", "TempDB","WorkoutProgramQuestions.json");
    private readonly string _trainerConversationPathFile = Path.Combine(AppContext.BaseDirectory, "../../../", "TempDB","trainerConversationQuestions.json");
   
    public List<WorkoutProgramQuestion>? GetWorkoutProgramQuestions()
    {
        string fullPath = Path.Combine(this._pathFile);
        fullPath = Path.GetFullPath(fullPath); 
        if (!File.Exists(fullPath)) return null;
        var jsonString = File.ReadAllText(fullPath);
        var questions = JsonSerializer.Deserialize<List<WorkoutProgramQuestion>>(jsonString, new JsonSerializerOptions { IncludeFields = true }) ?? new List<WorkoutProgramQuestion>();
        return questions;
    }

    public List<TrainerConversationQuestions>? GetTrainerConversationQuestions()
    {
        string fullPath = Path.Combine(this._trainerConversationPathFile);
        fullPath = Path.GetFullPath(fullPath); 
        if (!File.Exists(fullPath)) return null;
        var jsonString = File.ReadAllText(fullPath);
        var questions = JsonSerializer.Deserialize<List<TrainerConversationQuestions>>(jsonString, new JsonSerializerOptions { IncludeFields = true }) ?? new List<TrainerConversationQuestions>();
        return questions;
    }
}