namespace MyBot.Interfaces;

public interface ISurvey : IDisposable
{
    
    string? GetCurrentQuestion();
    
    void ToTheNextQuestion();

    List<string>? GetAnswers();
    
    void Dispose();
    
    protected virtual void Dispose(bool disposing){}
    
}