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

    string NormalizeAge(string input)
    {
        return input.Trim().Replace("–", "-"); 
    }
    
    public WorkoutProgram? GetWorkoutProgram(List<string> answers)
    {

        WorkoutProgram.Gender? gender;
        string? age;
        bool? isFitTestPassed;
        bool? isWidthOfWristTestPassed;
        WorkoutProgram? workoutProgram;
        switch (answers.Count)
        {
            case 3:
            {
                gender = Enum.Parse<WorkoutProgram.Gender>(answers[0]);
                age = answers[1];
                isFitTestPassed = answers[2] == "WK_test_true" ? true : false;
                workoutProgram = this._workoutPrograms.FirstOrDefault(wp => wp.UserGender == gender && wp.Age == age &&  wp.IsFitTestPassed == isFitTestPassed);

                if (workoutProgram != null) return workoutProgram;
                break;
            }
            case 4:
            {
                gender = Enum.Parse<WorkoutProgram.Gender>(answers[0]);
                
                age = NormalizeAge(answers[1]);
                isFitTestPassed = answers[2] == "WK_test_true";
                isWidthOfWristTestPassed = answers[3] == "Genetic_test_true";
                
                workoutProgram = _workoutPrograms.FirstOrDefault(wp =>
                    wp.UserGender == gender &&
                    NormalizeAge(wp.Age) == age &&
                    wp.IsFitTestPassed == isFitTestPassed &&
                    wp.IsWidthOfWristTestPassed == isWidthOfWristTestPassed
                );

                if (workoutProgram != null) return workoutProgram;
                break;
            }

        }
        return null;
    }

    public string? PrepareWorkoutProgramFile(List<string> answers)
    {
        string pathFile = Path.Combine(AppContext.BaseDirectory,"../../../", "FilesToSendToClients",$".WorkoutProgram_for_{answers[0]}_{answers[1]}.txt");
        string fullPath = Path.Combine(pathFile);
        fullPath = Path.GetFullPath(fullPath); 
        WorkoutProgram? workoutProgram = this.GetWorkoutProgram(answers);
        if(workoutProgram == null) return null;
        using (StreamWriter sw = new StreamWriter(fullPath, false))
        {
            foreach (var str in workoutProgram.WorkoutRecommendations)
            {
                sw.WriteLine(str);
            }
        }
        return fullPath; 
    }
}