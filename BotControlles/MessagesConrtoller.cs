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
    methodologicalMaterials = 5,
    conversationsWithTrainer = 6,
}

public class MessagesController : BaseController
{
    private Option[] _methodologicalMaterialsuserOptions = { new Option("програма тренувань для новачків", "/defineWorkoutProgram"),};
    private Option[] _conversationOptions =
    {
        new Option("💪 Тренувальний план", "/personalWKProgramm"),
        new Option("👨‍🏫 Персональне ведення", "/onlineManagement"),
        new Option("🎧 Консультація 1 на 1", "/onlineConsultation"),
        new Option("🥗 Раціон + підтримка", "/nutritionDietTracking")
    };
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
    

    protected override InlineKeyboardMarkup GetOptions(string? prefix, Option[] options)
    {
        var buttons = prefix is not null
            ? options
                .Select(o => new InlineKeyboardButton[]
                    { InlineKeyboardButton.WithCallbackData(o.route, $"{prefix}_{o.callback}") })
                .ToArray()
            : options.Select(o => new InlineKeyboardButton[]
                    { InlineKeyboardButton.WithCallbackData(o.route, $"{o.callback}") })
                .ToArray();

        return new InlineKeyboardMarkup(buttons);
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
            string name = user.FirstName + " " + user.LastName;
            string phone = route.Split('_').Last();
            botClientUser = new BotClientUser(chatId,  phone, name); 
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
                if (this._userService.isAdmin(user.Username))
                {
                    await this._botClient.SendMessage(
                        chat.Id,
                        "Choose option",
                        replyMarkup: this.GetOptions(null, this._adminOptions.ToArray())
                    
                    );
                    return;
                }
                await this._botClient.SendMessage(
                    chat.Id,
                    "Greeting message"
                );
                return;
            case BotCommands.methodologicalMaterials:
                await this._botClient.SendMessage(
                    chat.Id,
                    "Виберіть тему:",
                    replyMarkup:this.GetOptions(null, this._methodologicalMaterialsuserOptions)
                    );
                return;
            case BotCommands.textCalendar:
                if (this._userService.isAdmin(user.Username))
                {
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
                }
                
                return;
            
            case BotCommands.getKPI:
                if (this._userService.isAdmin(user.Username))
                {
                    await this._botClient.SendMessage(
                        chat.Id,
                        "Choose option:",
                        replyMarkup:this.GetOptions(null, this._adminOptions)
                    );
                    return;
                }
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
                
                this._userService.CreateBotClientUser(botClientUser.ChatId, botClientUser.Phone, botClientUser.Name);
                await this._botClient.SendMessage(chatId, "Дякую, ви успішно авторизовані.", replyMarkup: new ReplyKeyboardRemove());
                break;
            case BotCommands.conversationsWithTrainer:
            {
                await this._botClient.SendMessage(
                    chat.Id,
                    "Обери формат, який тобі підходить 👇\n\n" +
                    "💪 *Тренувальний план* — персональна програма під твою ціль\n" +
                    "👨‍🏫 *Персональне ведення* — супровід, контроль та підтримка\n" +
                    "🎧 *Консультація 1 на 1* — відповім на всі твої питання\n" +
                    "🥗 *Раціон + підтримка* — допоможу з харчуванням та трекінгом\n\n" +
                    "Натисни кнопку нижче 👇",
                    replyMarkup:this.GetOptions("conversation", this._conversationOptions.ToArray())
                );
                break;
            }
        }
        
    }
}