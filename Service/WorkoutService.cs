using MyBot.Classes;
using MyBot.Repository;

namespace MyBot.Service;

public class WorkoutService
{
    private readonly WorkoutProgramRepository  _workoutProgramRepository;
    private List<WorkoutProgram> _workoutPrograms;

    public WorkoutService(WorkoutProgramRepository workoutProgramRepository)
    {
        this._workoutProgramRepository = workoutProgramRepository;
        this._workoutPrograms = this._workoutProgramRepository.GetWorkoutPrograms() ?? [];
    }

    public List<WorkoutProgram> GetWorkoutPrograms()
    {
        return this._workoutPrograms;
    }

    public WorkoutProgram? GetWorkoutProgram(List<string> answers)
    {

        WorkoutProgram.Gender? gender;
        string? age;
        bool? isFitTestPassed;
        bool? isWidthOfWristTestPassed;
        WorkoutProgram? workoutProgram;
        if (answers.Count == 3)
        {
             gender = Enum.Parse<WorkoutProgram.Gender>(answers[0]);
             age = answers[1];
             isFitTestPassed = answers[2] == "WK_test_true" ? true : false;
             workoutProgram = this._workoutPrograms.FirstOrDefault(wp => wp.UserGender == gender && wp.Age == age &&  wp.IsFitTestPassed == isFitTestPassed);
             if (workoutProgram != null) return workoutProgram;
        }
        else if(answers.Count == 4)
        {
            gender = Enum.Parse<WorkoutProgram.Gender>(answers[0]);
            age = answers[1];
            isFitTestPassed = answers[2] == "WK_test_true" ? true : false;
            isWidthOfWristTestPassed = answers[3] == "Genetic_test_true" ? true : false;
            workoutProgram = this._workoutPrograms.FirstOrDefault(wp => wp.UserGender == gender && wp.Age == age &&  wp.IsFitTestPassed == isFitTestPassed &&  wp.IsWidthOfWristTestPassed == isWidthOfWristTestPassed);
            if (workoutProgram != null) return workoutProgram;
        }
        return null;
    }
}