namespace MyBot.Classes;

public class BotClientUser
{
    public long ChatId = long.MinValue;
    public string Phone  =  string.Empty;
    public string Name = string.Empty;
    
    public BotClientUser(long chatId, string phone, string name)
    {
        this.ChatId = chatId;
        this.Phone = phone;
        this.Name = name;
    }
    public BotClientUser(){}
}