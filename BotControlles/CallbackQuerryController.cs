using MyBot.Service;
using MyBot.State;
using Telegram.Bot;
using Telegram.Bot.Types;
using MyBot.State;
namespace MyBot.Controllers;

enum CallbackCommands
{
    loadUsersData = 0,
}
public class CallbackQueryController
{
    private string[] callbackRoutes = [];
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly UserIntendsState _userIntendsState;
    
    public CallbackQueryController(string[] callbackRoutes, ITelegramBotClient botClient, UserService userService, UserIntendsState userIntendsState)
    {
        this.callbackRoutes = callbackRoutes;
        this._botClient = botClient;
        this._userService = userService;
        this._userIntendsState = userIntendsState;
    }
    
    

    public async Task ListenRoutes(string route,Update update)
    {
        var callbackQuery = update.CallbackQuery;
        var message = callbackQuery.Message;
        var chat = callbackQuery.Message.Chat;
        var chatId = message.Chat.Id;
        var user = message.From;
        Console.WriteLine(route + " route");
        if (!Enum.TryParse(route, true, out CallbackCommands command)) 
        {
            Console.WriteLine($"Unknown route: {route}");
            return;
        };
         switch(command)
         {
             case CallbackCommands.loadUsersData:
             { 
                 // В этом типе клавиатуры обязательно нужно использовать следующий метод
                 await this._botClient.AnswerCallbackQuery(callbackQuery.Id, "Prepare the file to load."); 
                 // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку
                 this._userIntendsState.UserIntends[chat.Id] = UserIntends.PrepareLoadFile;
                 await this._botClient.SendMessage(
                     chat.Id,
                     $"Waiting a file...");
                 return;
             }
             
             // case "button3":
             // {
             //     // А тут мы добавили еще showAlert, чтобы отобразить пользователю полноценное окно
             //     await this._botClient.AnswerCallbackQuery(callbackQuery.Id, "А это полноэкранный текст!", showAlert: true);
             //     
             //     await this._botClient.SendMessage(chat.Id, 
             //         $"Вы нажали на {callbackQuery.Data}"
             //         );
             //     return;
             // }
        }
    }
}