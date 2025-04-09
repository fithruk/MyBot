using MyBot.Classes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using MyBot.Service;
using MyBot.State;
using User = MyBot.Classes.User;

namespace MyBot.Controllers;

enum BotCommands
{
    start = 0,
    textCalendar = 1,
    getKPI = 2,
    askPhone = 3,
    registrationClientUser = 4,
}

public class MessagesController : BaseController
{
    private Option[] _userOptions = { new Option("Workout Program Test", "/defineWorkoutProgram") };
    private Option[] _adminOptions = { new Option("Load data from file", "/loadUsersData"), new Option("Get info about client", "/getClientInfo"), new Option("Get KPi",  "/getKPI") };
    
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly UserIntendsState _userIntendsState;
    
    public MessagesController(
        ITelegramBotClient botClient, 
        UserService userService,
        UserIntendsState userIntendsState
        )
    {
        this._botClient = botClient;
        this._userService = userService;
        this._userIntendsState = userIntendsState;
    }
    

    protected override  InlineKeyboardMarkup GetProperlyOptions(string userName)
    {
        
        List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
        List<InlineKeyboardButton[]> userLayerButtons = new List<InlineKeyboardButton[]>()
        {
            this._userOptions.Select(o =>  InlineKeyboardButton.WithCallbackData(o.route, o.callback)).ToArray()
        };
        List<InlineKeyboardButton[]> adminLayerButtons = new List<InlineKeyboardButton[]>()
        {
            this._adminOptions.Select(o =>  InlineKeyboardButton.WithCallbackData(o.route, o.callback)).ToArray()
        };


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

    private async Task<string> DownloadFileAsync(Document document)
    {
        // Console.WriteLine("Текущая директория: " + Directory.GetCurrentDirectory());
        // Console.WriteLine(Environment.CurrentDirectory);
        var file = await this._botClient.GetFile(document.FileId);
        string localFilePath = Path.Combine("Downloads", document.FileName); 

        Directory.CreateDirectory("Downloads"); 

        await using var stream = File.Create(localFilePath);
        await this._botClient.DownloadFile(file.FilePath, stream);

        Console.WriteLine($"Файл загружен в: {localFilePath}");
        return localFilePath;
    }
    
    public override async  Task ListenRoutes(string route, Update update)
    {
        BotClientUser? botClientUser = null;
        var message = update.Message;
        var chat = message.Chat;
        long chatId = message.Chat.Id;
        var user = message.From;
        Console.WriteLine($"Пришло сообщение! from {user.Username} {user.LastName} chat id {chatId}");

        if (route.StartsWith("phone") )
        {
            string phone = route.Split('_').Last();
            botClientUser = new BotClientUser(chatId,  phone);
            route = "registrationClientUser";
        }
        
        
        if (!Enum.TryParse(route, true, out BotCommands command))
        {
            Console.WriteLine($"Unknown route: {route}");
            return;
        };

        if (!this._userService.IsClientUserExist(chatId))
        {
            command = BotCommands.askPhone;
        }

        if (botClientUser is not null)
        {
            command = BotCommands.registrationClientUser;
        }
        
        
        this._botClient.SendMessage(chat.Id, "", replyMarkup: new ReplyKeyboardRemove()); // ---------------------
        Console.WriteLine($"command: {command}");
        switch (command)
        {
            case BotCommands.start:
                await this._botClient.SendMessage(
                    chat.Id,
                    "Choose option:",
                    replyMarkup:this.GetProperlyOptions(user.Username)
                    );
                return;
            case BotCommands.textCalendar:
                var document = message.Document;
                if (
                    this._userIntendsState.UserIntends.TryGetValue(message.From.Id, 
                        out UserIntends userIntends) && userIntends == UserIntends.PrepareLoadFile)
                {
                    string filePath = await this.DownloadFileAsync(document); 
                    this._userIntendsState.UserIntends[user.Id] = UserIntends.Default;
                    ReadCalendarFile readCalendarFile = new ReadCalendarFile(filePath);
                    var dates = readCalendarFile.LoadUsers();
                    foreach (var i in dates)
                    {
                        
                        User newUser = new User( i.Key, i.Value);
                        this._userService.AddNewUser(newUser);
                    }
                    this._userService.SaveUsersInDB();
                    await this._botClient.SendMessage(chat.Id, "File successfully saved!");
                    this._userService.SaveEvents(readCalendarFile.GetEvents().ToList());
                }
                
                return;
            
            case BotCommands.getKPI:
                await this._botClient.SendMessage(
                    chat.Id,
                    "Choose option:",
                    replyMarkup:this.GetProperlyOptions(user.Username)
                );
                return;
            
            case BotCommands.askPhone:
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("📱 Надіслати номер") { RequestContact = true }
                })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                };
                await this._botClient.SendMessage(
                    chatId: chat.Id,
                    text: "Будь ласка, надішліть свій номер телефону:",
                    replyMarkup: replyKeyboard
                );
                break;
            
               case BotCommands.registrationClientUser:
                if (botClientUser is null)
                {
                    Console.WriteLine("Ошибка: botClientUser не инициализирован.");
                    break;
                }
                
                this._userService.CreateBotClientUser(botClientUser.ChatId, botClientUser.Phone);
                await this._botClient.SendMessage(chatId, "Дякую, ви успішно авторизовані.", replyMarkup: new ReplyKeyboardRemove());
                break;
        }
    }
}