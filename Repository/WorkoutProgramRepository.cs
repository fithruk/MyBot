using MyBot.Classes;
using MyBot.MyBotUsersDBJson;

namespace MyBot.Repository;

public class WorkoutProgramRepository
{
    private readonly WorkoutProgramsDB _workoutPrograms;

    public WorkoutProgramRepository(WorkoutProgramsDB workoutPrograms)
    {
        this._workoutPrograms = workoutPrograms;
    }
    
    public List<WorkoutProgram>? GetWorkoutPrograms()
    {
        return this._workoutPrograms.GetWorkoutPrograms();
    }  
}