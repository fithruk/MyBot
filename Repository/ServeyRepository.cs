using MyBot.Classes;
using MyBot.MyBotUsersDBJson;

namespace MyBot.Repository;

public class SurveyRepository
{
    private readonly SurveyQuestionsDB _surveyQuestions;
    

    public SurveyRepository(SurveyQuestionsDB surveyQuestions, WorkoutProgramsDB  workoutPrograms)
    {
        this._surveyQuestions = surveyQuestions;
      
    }

    public  List<WorkoutProgramQuestion>? GetWorkoutProgramQuestions()
    {
        return this._surveyQuestions.GetWorkoutProgramQuestions();
    }

    public List<TrainerConversationQuestions>? GetTrainerConversationQuestions()
    {
        return this._surveyQuestions.GetTrainerConversationQuestions();
    }
}