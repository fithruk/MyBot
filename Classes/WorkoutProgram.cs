namespace MyBot.Classes;

public class WorkoutProgram
{
    public enum Gender
    {
        male,
        female
    }
    
    public bool IsFitTestPassed;
    public bool? IsWidthOfWristTestPassed;
    public string Age;
    public Gender UserGender;

    public List<string> WorkoutRecommendations;

    public WorkoutProgram(bool isFitTestPassed, bool isWidthOfWristTestPassed, string age, Gender gender,  List<string> workoutRecommendations)
    {
        this.IsFitTestPassed = isFitTestPassed;
        this.IsWidthOfWristTestPassed = isWidthOfWristTestPassed;
        this.Age = age;
        this.UserGender = gender;
        this.WorkoutRecommendations = workoutRecommendations;
    }
    
    public WorkoutProgram(bool isFitTestPassed,  string age, Gender gender,  List<string> workoutRecommendations)
    {
        this.IsFitTestPassed = isFitTestPassed;
        this.Age = age;
        this.UserGender = gender;
        this.WorkoutRecommendations = workoutRecommendations;
    }
    
    public WorkoutProgram(){}
}