using MyBot.Classes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using MyBot.Service;
using MyBot.State;

namespace MyBot.Controllers;

enum BotCommands
{
    start = 0,
    textCalendar = 1,
}

public class MessagesController
{
    private string[] _routes;
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly UserIntendsState _userIntendsState;
    
    public MessagesController(
        string[] routes, 
        ITelegramBotClient botClient, 
        UserService userService,
        UserIntendsState userIntendsState
        )
    {
        this._routes = routes;
        this._botClient = botClient;
        this._userService = userService;
        this._userIntendsState = userIntendsState;
    }
    
    // await botClient.SendMessage(
    //     chat.Id,
    //     message.Text, // отправляем то, что написал пользователь
    //     replyParameters: message.MessageId // по желанию можем поставить этот параметр, отвечающий за "ответ" на сообщение
    // );


    // private string GetProperlyCommands(string userName)
    // {
    //     string text = _userService.isAdmin(userName)  ? "Choose option:\n" +
    //                   "/inline\n" +
    //                   "/loadCalendarFile\n" 
    //         : "Выбери клавиатуру:\n" +
    //           "/inline\n";
    //     return text;
    // }

    private InlineKeyboardMarkup GetProperlyOptions(string userName)
    {
        
        List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
        List<InlineKeyboardButton[]> userLayerButtons = new List<InlineKeyboardButton[]>()
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("А это просто кнопка", "button1")
            }
        };
        List<InlineKeyboardButton[]> adminLayerButtons = new List<InlineKeyboardButton[]>()
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Load data from file", "/loadUsersData")
            }
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
        string localFilePath = Path.Combine("Downloads", document.FileName); // Локальный путь

        Directory.CreateDirectory("Downloads"); // Убедимся, что папка существует

        await using var stream = File.Create(localFilePath);
        await this._botClient.DownloadFile(file.FilePath, stream);

        Console.WriteLine($"Файл загружен в: {localFilePath}");
        return localFilePath;
    }
    
    public async Task listenRouts(string route, Update update)
    {
        
        var message = update.Message;
        var chat = message.Chat;
        var user = message.From;
        Console.WriteLine($"Пришло сообщение! from {user.Username} {user.LastName}");
        
        if (!Enum.TryParse(route, true, out BotCommands command))
        {
            Console.WriteLine($"Unknown route: {route}");
            return;
        };
        this._botClient.SendMessage(chat.Id, "", replyMarkup: new ReplyKeyboardRemove());
        switch (command)
        {
            case BotCommands.start:
                Console.WriteLine($"Route: {route}");
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
                    readCalendarFile.PrintEvents();
                }
                return;
        }
    }
}