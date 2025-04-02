using MyBot.Service;
using MyBot.State;
using Telegram.Bot;
using MyBot.Classes;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using User = MyBot.Classes.User;

namespace MyBot.Controllers;

enum CallbackCommands
{
    loadUsersData = 0,
    getClientInfo = 1,
    sendClientInfo = 2,
    getKPI = 4,
}
public class CallbackQueryBaseController : BaseController
{
    
    private Option[] _userOptions = { new Option("", "") };
    private Option[] _adminOptions = { new Option("", "") };
    private List<Option> _usersNamesOptions;
    
    private string[] callbackRoutes = [];
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly UserIntendsState _userIntendsState;
    
    public CallbackQueryBaseController(string[] callbackRoutes, ITelegramBotClient botClient, UserService userService, UserIntendsState userIntendsState)
    {
        this.callbackRoutes = callbackRoutes;
        this._botClient = botClient;
        this._userService = userService;
        this._userIntendsState = userIntendsState;
        this._usersNamesOptions = this._userService.GetUsers().Select(u => new Option(u.Name, u.Name)).ToList();
    }


    protected override InlineKeyboardMarkup GetProperlyOptions(string userName)
    {
        List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
        List<InlineKeyboardButton[]> userLayerButtons = new List<InlineKeyboardButton[]>()
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("А это просто кнопка", "button1")
            }
        };
        List<InlineKeyboardButton[]> adminLayerButtons = this._usersNamesOptions
            .Select(o => new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(o.route, $"user_{o.callback}") })
            .ToList();


        if (_userService.isAdmin(userName))
        {
            
            inlineKeyboardButtons.AddRange(adminLayerButtons);
        }
        else
        {
            inlineKeyboardButtons.AddRange(userLayerButtons);
        }
        
        return new InlineKeyboardMarkup(inlineKeyboardButtons);
    }

    public override async Task ListenRoutes(string route,Update update)
    {
        MyBot.Classes.User userInfo = new User();
        var callbackQuery = update.CallbackQuery;
        var message = callbackQuery.Message;
        var chat = callbackQuery.Message.Chat;
        Console.WriteLine(route + " route");
        if (callbackQuery.Data.StartsWith("user_"))
        {
            var userName = callbackQuery.Data.Split('_')[1];
            userInfo = this._userService.GetUser(userName);
            Console.WriteLine(userName);
            route = "sendClientInfo";
        }
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
                 // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку
                 await this._botClient.AnswerCallbackQuery(callbackQuery.Id, "Prepare the file to load."); 
                
                 this._userIntendsState.UserIntends[chat.Id] = UserIntends.PrepareLoadFile;
                 await this._botClient.SendMessage(
                     chat.Id,
                     $"Waiting a file...");
                 return;
             }
             case CallbackCommands.getClientInfo:
                 await this._botClient.AnswerCallbackQuery(callbackQuery.Id, "Choose client to load information."); 
                 await this._botClient.SendMessage(
                     chat.Id,
                     $"Choose user to load info",
                     replyMarkup:this.GetProperlyOptions(chat.Username));
                 return;
             
             case CallbackCommands.sendClientInfo:
             {
                 // А тут мы добавили еще showAlert, чтобы отобразить пользователю полноценное окно
                
                 if (userInfo != null)
                 {
                     string answerString = $"Имя: {userInfo.Name}\n" +
                                           $"Тренировки:\n{string.Join("\n", userInfo.DatesOfWorkouts.Select(w => $"- {w:dd.MM.yyyy HH:mm}"))}\n" +
                                           $"Всего тренировок: {userInfo.DatesOfWorkouts.Count}";
                     
                     Console.WriteLine(answerString);
                     await this._botClient.AnswerCallbackQuery(callbackQuery.Id); // answerString ,showAlert: true
                     await this._botClient.SendMessage(chat.Id,
                         answerString
                     );

                     Console.WriteLine("Сообщение отправлено успешно!");
                 }

                 return;
             }
             case CallbackCommands.getKPI:
             {
                 await this._botClient.AnswerCallbackQuery(callbackQuery.Id);
                 Console.WriteLine(route + " get KPI");

                 var events = this._userService.GetEvents();
                 var KPI = new KPIStat(events);
                 string outputPath = KPI.GetKPIMonthByMonth();

                 await using Stream stream = File.OpenRead(outputPath);
                 InputFileStream document = new InputFileStream(stream, "CPI.txt");

                 await this._botClient.SendDocument(
                     chatId: chat.Id,
                     document: document,
                     caption: "Отчет по CPI"
                 );

                 return;
             }
        }
    }
}