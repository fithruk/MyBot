using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyBot.Controllers;

public abstract class BaseController
{
    
    protected struct Option
    {
        public string route;
        public string callback;

        public Option(string route, string callback)
        {
            this.route = route;
            this.callback = callback;
        }
    }
    protected  abstract InlineKeyboardMarkup GetOptions(string? prefix ,Option[] options);
    
    public  abstract Task ListenRoutes(string route, Update update);
}