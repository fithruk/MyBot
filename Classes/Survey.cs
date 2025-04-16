using MyBot.Interfaces;
using MyBot.Classes;
namespace MyBot.Classes;

// Questions in array, string[]
// int current question
// set, get current question
// collection for save answers
// set, get for answers collection
// detach method for removing survey

public class Survey : ISurvey
{
    // private Dictionary<string, string?> _transliteratedKeys = new Dictionary<string, string?>() {
    //     { "чоловік", "male" },
    //     { "11-14", "11-14" },
    //     { "15-18", "15-18" },
    //     { "19-28", "19-28" },
    //     { "29-39", "29-39" },
    //     { "40 +", "40 +" },
    //     { "жінка", "female" },
    //     { "15-19", "15-19" },
    //     { "20-29", "20-29" },
    //     { "30-35", "30-35" },
    //     { "36-44", "36-44" },
    //     { "44 +", "44 +" },
    //     {"Я поки що не змогла.", "WK_test_false"},
    //     {"Так. Я це змогла зробити.", "WK_test_true"},
    //     {"Я поки що не зміг це зробити.", "WK_test_false"},
    //     {"Я зміг це зробити.", "WK_test_true"},
    //     {"НІ. Довжини пальців забракло.", "Genetic_test_false"},
    //     {"ТАК. Можу.", "Genetic_test_true"},
    // };
    
    
    
    private bool _disposed = false;
    
    private List<WorkoutProgramQuestion>? _questions;
    private int? _currentQuestionInd;
    private HashSet<string>? _answers;

    public Survey(List<WorkoutProgramQuestion> questions)
    {
        this._questions = questions;
        this._answers = new ();
        this._currentQuestionInd = 0;
    }


    public string? GetCurrentQuestion()
    {
        if (this._currentQuestionInd.HasValue && this._questions != null)
        {
            return this._questions.ElementAtOrDefault(this._currentQuestionInd.Value).Question;
        }
        return null;
    }

 
    public string? GetCaption() 
    {
        if (this._currentQuestionInd.HasValue && this._questions != null)
        {
            return this._questions.ElementAtOrDefault(this._currentQuestionInd.Value)?.Caption;
        }
        return null;
    }
    
   
    public List<string> GetAnswerOptions()
    {
        if (this._currentQuestionInd.HasValue && this._questions != null)
        {
            return this._questions.ElementAtOrDefault(this._currentQuestionInd.Value).Options.ToList();
        }
        return null;
    }
    

    public void ToTheNextQuestion()
    {
        this._currentQuestionInd = this._questions?.FindIndex(question =>
            question.Keys.Length > 0 &&
            question.Keys.All(key => this._answers.Contains(key)) &&
            this._answers.All(ans => question.Keys.Contains(ans)));
    }

    public List<string>? GetAnswers()
    {
        return (this._answers ?? []).ToList();
    }

    public void AddAnswer(string answer)
    {
        var properlyKey = SurveyKeys.GetWkProgramQuestionsKeys()[answer];
        if (properlyKey != null) this._answers?.Add(properlyKey);
    }

    public void Dispose()
    {
       Dispose(true);
       GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if(!this._disposed) return;

        if (disposing)
        {
            this._questions = null;
            this._answers = null;
        }
        
        this._currentQuestionInd = null;
        this._disposed = true;
    }

    ~Survey()
    {
        Dispose(false);
    }
}