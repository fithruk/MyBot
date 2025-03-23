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
}