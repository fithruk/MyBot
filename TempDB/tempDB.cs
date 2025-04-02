using System.Runtime.CompilerServices;
using System.Text.Json;
using Ical.Net.CalendarComponents;
using MyBot.Classes;

namespace MyBot.MyBotUsersDBJson;

public class TempUsersDB
{
    private static List<User> _usersArray = new ();
    private static List<User> _adminsArray;
    private static List<CalendarEvent>  _eventsArray = new ();
    
    // public static void Serialize()
    // {
    //    string jsonString = JsonSerializer.Serialize(_usersArray);
    //    string path = @"/Users/admin/RiderProjects/MyBot/MyBot/TempDB/usersDB.json";
    //    File.WriteAllText(path, jsonString); 
    // }
    //
    // public static void Deserialize()
    // {
    //     string path = @"/Users/admin/RiderProjects/MyBot/MyBot/TempDB/usersDB.json";
    //     if (File.Exists(path))
    //     {
    //         try
    //         {
    //             string jsonString = File.ReadAllText(path);
    //             _usersArray = JsonSerializer.Deserialize<List<User>>(jsonString) ?? new List<User>();
    //         }
    //         catch (Exception e)
    //         {
    //             Console.WriteLine(e);
    //             throw;
    //         }
    //     }
    // }

    public List<User> getUsers()
    {
        return _usersArray;
    }

    public User getUser(string name)
    {
        User user = _usersArray.Find(u => u.Name == name);
        
        return user;
    }

    public void addUser(User user)
    {
        _usersArray.Add(user);
    }

    public void SaveUsersInDB()
    {
        string path = @"C:\Users\User\source\repos\fithruk\MyBot\TempDB\usersDB.json";
        File.WriteAllText(path, JsonSerializer.Serialize(_usersArray,new JsonSerializerOptions(){IncludeFields = true}));
    }

    public bool isAdmin(string userName)
    {
        string path = @"C:\Users\User\source\repos\fithruk\MyBot\TempDB\admins.json";
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

    public void SaveEvents(List<CalendarEvent> events)
    {
        _eventsArray.AddRange(events);
    }

    public List<CalendarEvent> GetEvents()
    {
        return _eventsArray;
    }

    public void CreateNewClientUser(long chatId, string phone)
    {
        var newClient = new BotClientUser(chatId, phone);
        string path = @"C:\Users\User\source\repos\fithruk\MyBot\TempDB\botClientUsers.json";


        if (!File.Exists(path))
        {
          
            var newArr = new List<BotClientUser> { newClient };
            File.WriteAllText(path, JsonSerializer.Serialize(newArr, new JsonSerializerOptions { IncludeFields = true }));
        }
        else
        {
            var arr = JsonSerializer.Deserialize<List<BotClientUser>>(File.ReadAllText(path), new JsonSerializerOptions { IncludeFields = true }) ?? new List<BotClientUser>();
            arr.Add(newClient);
            File.WriteAllText(path, JsonSerializer.Serialize(arr, new JsonSerializerOptions { IncludeFields = true }));
        }
    }

    public bool IsClientUserExist(long chatId)
    {
        string path = @"C:\Users\User\source\repos\fithruk\MyBot\TempDB\botClientUsers.json";
        List<BotClientUser> clients = JsonSerializer.Deserialize<List<BotClientUser>>(File.ReadAllText(path), new JsonSerializerOptions(){IncludeFields = true})!;
        return clients.Exists(u => u.ChatId == chatId && u.Phone != "" );
    }
}