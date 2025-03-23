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

    public List<User> GetUsers()
    {
        return _tempUsersDB.getUsers();
    }

    public bool isAdmin(string userName)
    {
        return _tempUsersDB.isAdmin(userName);
    }
}