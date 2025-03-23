using System.Text.Json.Serialization;

namespace MyBot.Classes;

public class User
{
    public enum Role
    {
        User,
        Admin
    }
    //[{"Username": "Death_rowww", "FirstName":"Sergey", "LastName":"","datesOfWorkouts":[], "UserRole": 1}]
    public string Username { get; private set; } = string.Empty;
    public string FirstName  { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public List<DateTime> DatesOfWorkouts = new List<DateTime>();
    public Role UserRole = Role.User;

    [JsonConstructor] 
    public User(string username, string firstName, string lastName, List<DateTime> datesOfWorkouts, Role userRole)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        datesOfWorkouts = datesOfWorkouts ?? new List<DateTime>(); 
        UserRole = userRole;
    }
    
   public User(){}
    
    
}