namespace MyBot.Classes;

public class BotClientUser
{
    public long ChatId = long.MinValue;
    public string Phone  =  string.Empty;
    
    public BotClientUser(long chatId, string phone)
    {
        this.ChatId = chatId;
        this.Phone = phone;
    }
    public BotClientUser(){}
}