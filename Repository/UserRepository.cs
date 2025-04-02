using Ical.Net.CalendarComponents;
using MyBot.Classes;
using MyBot.MyBotUsersDBJson;
namespace MyBot.Repository;

public class UserRepository
{
    private readonly TempUsersDB _tempUsersDB;

    public UserRepository(TempUsersDB tempUsersDB)
    {
        _tempUsersDB = tempUsersDB;
    }

    public void AddUser(User user)
    {
        _tempUsersDB.addUser(user);
    }

    public void SaveUsersInDB()
    {
        this._tempUsersDB.SaveUsersInDB();
    }

    public User GetUser(string name)
    {
       return this._tempUsersDB.getUser(name);
    }
    
    public List<User> GetUsers()
    {
        return _tempUsersDB.getUsers();
    }

    public bool isAdmin(string userName)
    {
        return _tempUsersDB.isAdmin(userName);
    }

    public void SaveEvents(List<CalendarEvent> events)
    {
        this._tempUsersDB.SaveEvents(events);
    }
    
    public List<CalendarEvent> GetEvents()
    {
        return _tempUsersDB.GetEvents();
    }

    public void CreateBotClientUser(long chatId, string phone)
    {
        this._tempUsersDB.CreateNewClientUser(chatId, phone);
    }

    public bool IsClientUserExist(long chatId)
    {
        return this._tempUsersDB.IsClientUserExist(chatId);
    }
}