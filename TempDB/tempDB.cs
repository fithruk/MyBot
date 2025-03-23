using System.Runtime.CompilerServices;
using System.Text.Json;
using MyBot.Classes;

namespace MyBot.MyBotUsersDBJson;

public class TempUsersDB
{
    private static List<User> _usersArray = new List<User>();
    private static List<User> _adminsArray;
    
    public static void Serialize()
    {
       string jsonString = JsonSerializer.Serialize(_usersArray);
       string path = @"/Users/admin/RiderProjects/MyBot/MyBot/TempDB/usersDB.json";
       File.WriteAllText(path, jsonString); 
    }

    public static void Deserialize()
    {
        string path = @"/Users/admin/RiderProjects/MyBot/MyBot/TempDB/usersDB.json";
        if (File.Exists(path))
        {
            try
            {
                string jsonString = File.ReadAllText(path);
                _usersArray = JsonSerializer.Deserialize<List<User>>(jsonString) ?? new List<User>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public List<User> getUsers()
    {
        return _usersArray;
    }

    public void addUser(User user)
    {
        _usersArray.Add(user);
    }

    public bool isAdmin(string userName)
    {
        string path = @"/Users/admin/RiderProjects/MyBot/MyBot/TempDB/admins.json";
        if (File.Exists(path))
        {
            try
            {
                string jsonString = File.ReadAllText(path);
                _adminsArray = JsonSerializer.Deserialize<List<User>>(jsonString , new JsonSerializerOptions(){IncludeFields = true})!;
                
                bool isAdmin = _adminsArray.Exists(u => u.Username == userName);
                Console.WriteLine($"User {userName} is an administrator? {isAdmin}");
                return isAdmin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        return false;
    }
}