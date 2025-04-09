namespace MyBot.Classes;



public class WorkoutProgramQuestion
{
    public string[] Keys { get; set; }
    public string Caption { get; set; }
    public string Question { get; set; }
    public string[] Options { get; set; }

   
    public WorkoutProgramQuestion() {}

   
    public WorkoutProgramQuestion(string[] keys, string caption, string question, string[] options)
    {
        this.Keys = keys;
        this.Caption = caption;
        this.Question = question;
        this.Options = options;
    }
}