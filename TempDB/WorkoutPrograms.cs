using System.Text.Json;
using System.Text.Json.Serialization;
using MyBot.Classes;

namespace MyBot.MyBotUsersDBJson;

public class WorkoutProgramsDB
{
    private readonly string _pathFile = Path.Combine(AppContext.BaseDirectory,"../../../", "TempDB","WorkoutPrograms.json");
    
    public List<WorkoutProgram>? GetWorkoutPrograms()
    {
        string fullPath = Path.Combine(this._pathFile);
        fullPath = Path.GetFullPath(fullPath); 
        if (!File.Exists(fullPath)) return null;
        var jsonString = File.ReadAllText(fullPath);
        var workoutPrograms = JsonSerializer.Deserialize<List<WorkoutProgram>>(jsonString, new JsonSerializerOptions { 
            IncludeFields = true , 
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }}) ?? [];
        return workoutPrograms;
    } 
}