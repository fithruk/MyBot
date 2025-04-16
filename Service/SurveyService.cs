using MyBot.Classes;
using MyBot.Interfaces;
using MyBot.Repository;

namespace MyBot.Service;

public class WorkoutSurveyService : ISurveyService<WorkoutProgramQuestion>
{
    private readonly SurveyRepository _surveyRepository;

    private Dictionary<long, Survey> _surveys = new();
    private Dictionary<long, List<int>> _messageIds = new();

    public WorkoutSurveyService(SurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public void AddMessage(long chatId, int messageId)
    {
        if (this._messageIds.ContainsKey(chatId))
        {
            this._messageIds[chatId].Add(messageId);
            return;
        }

        this._messageIds.TryAdd(chatId, new List<int>(){messageId});
    }
    

    public List<int>? GetMessageIds(long chatId)
    {
        return this._messageIds.TryGetValue(chatId, out var id) ? id : null;
    }
    
    
    public List<WorkoutProgramQuestion>? GetQuestions()
    {
        return this._surveyRepository.GetWorkoutProgramQuestions();
    }

    public void CreateNewSurvey(long chatId, List<WorkoutProgramQuestion> questions)
    {
        if (this._surveys.ContainsKey(chatId))  return;
        this._surveys.TryAdd(chatId, new Survey(questions));
    }

    public Survey? GetSurvey(long chatId)
    {
        return this._surveys.GetValueOrDefault(chatId);
    }

    public void SaveUserSurveyAnswer(long chatId, string answer)
    {
        this._surveys.GetValueOrDefault(chatId)?.AddAnswer(answer);
    }
    
    
    public void DeleteSurvey(long chatId){
        this._surveys[chatId].Dispose();
        this._surveys.Remove(chatId);
    }

}