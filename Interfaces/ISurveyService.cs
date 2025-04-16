using MyBot.Classes;

namespace MyBot.Interfaces;

public interface ISurveyService<T>
{

    void AddMessage(long chatId, int messageId);



    public List<int>? GetMessageIds(long chatId);


    
    List<T>? GetQuestions();


    void CreateNewSurvey(long chatId, List<WorkoutProgramQuestion> questions);


    Survey? GetSurvey(long chatId);

    void SaveUserSurveyAnswer(long chatId, string answer);



    void DeleteSurvey(long chatId);
}