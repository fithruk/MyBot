using Ical.Net.CalendarComponents;
using MyBot.Classes;
using MyBot.Repository;
namespace MyBot.Service;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool isAdmin(string userName)
    {
        return _userRepository.isAdmin(userName);
    }

    public void AddNewUser(User user)
    {
        this._userRepository.AddUser(user);
    }

    public void SaveUsersInDB()
    {
        this._userRepository.SaveUsersInDB();
    }

    public List<User> GetUsers()
    {
        return this._userRepository.GetUsers();
    }

    public User GetUser(string name)
    {
        return this._userRepository.GetUser(name);
    }

    public void SaveEvents(List<CalendarEvent> events)
    {
        this._userRepository.SaveEvents(events);
    }
    
    public List<CalendarEvent> GetEvents ()
    {
        return this._userRepository.GetEvents();
    }

    public void CreateBotClientUser(long chatId, string phone)
    {
        this._userRepository.CreateBotClientUser(chatId, phone);
    }

    public bool IsClientUserExist(long chatId)
    {
        return this._userRepository.IsClientUserExist(chatId);
    }
}