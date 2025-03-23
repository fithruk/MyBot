namespace MyBot.State;

public enum UserIntends
{ 
    Default,
    PrepareLoadFile
}

public class UserIntendsState
{
    public Dictionary<long, UserIntends> UserIntends = new();
}