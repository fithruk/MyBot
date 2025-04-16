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
    defineWorkoutProgram = 5,
    conversation = 6,
}
public class CallbackQueryBaseController : BaseController
{
    private Dictionary<long, int> _lastMessageIds = new(); // chatId -> messageId
    
    private List<Option> _userOptions = [new Option("", "")];
    private Option[] _adminOptions = [new Option("", "")];
    private readonly List<Option> _usersNamesOptions;
    
   
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly UserIntendsState _userIntendsState;
    private readonly WorkoutSurveyService _surveyService;
    private readonly WorkoutService _workoutService;
    
    public CallbackQueryBaseController(ITelegramBotClient botClient, UserService userService, UserIntendsState userIntendsState, WorkoutSurveyService surveyService,  WorkoutService workoutService)
    {
      
        this._botClient = botClient;
        this._userService = userService;
        this._userIntendsState = userIntendsState;
        this._usersNamesOptions = this._userService.GetUsers().Select(u => new Option(u.Name, u.Name)).ToList();
        this._surveyService = surveyService;
        this._workoutService = workoutService;
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

    public override async Task ListenRoutes(string route,Update update)
    {
        MyBot.Classes.User userInfo = new User();
        var callbackQuery = update.CallbackQuery;
        var message = callbackQuery.Message;
        var chat = callbackQuery.Message.Chat;
       
        if (callbackQuery.Data != null && callbackQuery.Data.StartsWith("user_"))
        {
            var userName = callbackQuery.Data.Split('_')[1];
            userInfo = this._userService.GetUser(userName);
            Console.WriteLine(userName);
            route = "sendClientInfo";
        }
        
        if(callbackQuery.Data != null && callbackQuery.Data.StartsWith("survey_"))
        {
            var survey = this._surveyService.GetSurvey(chat.Id);
            if(survey == null) return;
            var answer = callbackQuery.Data.Split('_').Last();
            survey.AddAnswer(answer);
            survey.ToTheNextQuestion();
            route = "defineWorkoutProgram";
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
             {
                 await this._botClient.AnswerCallbackQuery(callbackQuery.Id, "Choose client to load information."); 
                 await this._botClient.SendMessage(
                     chat.Id,
                     $"Choose user to load info",
                     replyMarkup:this.GetOptions("user",this._usersNamesOptions.ToArray()));

                 Console.WriteLine("Массив клиентов отправлен на клиент!");
                 return;
             }
             
             case CallbackCommands.sendClientInfo:
             {
                 // А тут мы добавили еще showAlert, чтобы отобразить пользователю полноценное окно
                if(!this._userService.isAdmin(chat.Username)) return;
                 if (userInfo != null)
                 {
                     string answerString = $"Имя: {userInfo.Name}\n" +
                                           $"Тренировки:\n{string.Join("\n", userInfo.DatesOfWorkouts.Select(w => $"- {w:dd.MM.yyyy HH:mm}"))}\n" +
                                           $"Всего тренировок: {userInfo.DatesOfWorkouts.Count}";
                     
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
                 if(!this._userService.isAdmin(chat.Username)) return;
                 
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
                 
                 Console.WriteLine("Отчет по CPI отправлен успешно!");
                 return;
             }
             case CallbackCommands.defineWorkoutProgram:
             {
                 try
                 {
                     var questions = this._surveyService.GetQuestions();
                     if (questions == null) return;
                     
                     this._surveyService.CreateNewSurvey(chat.Id, questions);
                     await this._botClient.AnswerCallbackQuery(callbackQuery.Id,
                         "Тут короче надо как то предукпредить о дальнейших действиях");

                     var ids = this._surveyService.GetMessageIds(chat.Id);
                     
                     if(ids != null)
                     {
                         foreach (var messageId in ids)
                         {
                             try
                             {
                                 await this._botClient.DeleteMessage(chat.Id, messageId);
                             }
                             catch (Telegram.Bot.Exceptions.ApiRequestException ex) when (ex.Message.Contains("message to delete not found"))
                             {
                                 continue;
                             }
                             catch (Exception ex)
                             {
                                 Console.WriteLine($"Не удалось удалить сообщение {messageId}: {ex.Message}");
                             }
                         }
                     };
                    var msg_1 =  await this._botClient.SendMessage(chat.Id, this._surveyService.GetSurvey(chat.Id).GetCaption(), ParseMode.Html);
                     this._userOptions.Clear();
                     this._userOptions = this._surveyService.GetSurvey(chat.Id).GetAnswerOptions()
                         .Select(op => new Option(op, $"survey_{op}")).ToList();
                     var msg_2 = await this._botClient.SendMessage(chat.Id, this._surveyService.GetSurvey(chat.Id).GetCurrentQuestion(),
                         replyMarkup: this.GetOptions(null,this._userOptions.ToArray()), parseMode:ParseMode.Html);
                     Console.WriteLine("Опрос в процессе, данные обрабатываются");
                     this._surveyService.AddMessage(chat.Id, msg_1.MessageId);
                     this._surveyService.AddMessage(chat.Id, msg_2.MessageId);
                     return;
                 }
                 catch (Exception e)
                 {
                     Console.WriteLine(e);
                     var wpFilePath = this._workoutService.PrepareWorkoutProgramFile(this._surveyService.GetSurvey(chat.Id).GetAnswers());
                     Console.WriteLine("Опрос успешно закончен, идет подготовка файла");
                     if (wpFilePath == null)
                     {
                         await this._botClient.SendMessage(chat.Id, "Помилка в логіці опитування, почніть спочатку.");
                         this._surveyService.DeleteSurvey(chat.Id);
                         return;
                     };
                    if (!File.Exists(wpFilePath)) File.Create(wpFilePath);
                     await using Stream stream = File.OpenRead(wpFilePath);
                     InputFileStream document = new InputFileStream(stream, $"{wpFilePath.Split('.')[1]}.txt");

                     await this._botClient.SendDocument(
                         chatId: chat.Id,
                         document: document,
                         caption: "Програма тренувань готова"
                     );
                 
                     Console.WriteLine("Программа тренировок отправлена успешно!");
                     this._surveyService.DeleteSurvey(chat.Id);
                     throw;
                     
                 }
             }
             case CallbackCommands.conversation:
             {
                 try
                 {
                     Console.WriteLine(callbackQuery.Data + " conversation");
                    
                     var questions = this._surveyService.GetQuestions();
                     if (questions == null) return;
                     
                     return;
                 }
                 catch (Exception e)
                 {
                     Console.WriteLine(e);
                     throw;
                 }
                 
             }
        }
    }
}