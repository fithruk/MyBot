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
    
    public string Name { get; private set; } = string.Empty;
    
    public List<DateTime> DatesOfWorkouts = new ();
    public Role UserRole = Role.User;

    [JsonConstructor] 
    public User(string username, string name, List<DateTime> datesOfWorkouts, Role userRole)
    {
        
        Username = username;
        Name = name;
        DatesOfWorkouts = datesOfWorkouts ?? new List<DateTime>(); 
        UserRole = userRole;
    }

    public User(string name, List<DateTime> datesOfWorkouts)
    {
        Name = name;
        DatesOfWorkouts = datesOfWorkouts ?? new List<DateTime>();
    }
    
   public User(){}
    
    
}