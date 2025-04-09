using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MyBot.Controllers;
using MyBot.Repository;
using MyBot.Service;
using MyBot.State;


namespace MyBot.Classes;

public static class BotHandler
{
    public static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var userService = ServiceLocator.GetService<UserService>();
        var surveyService = ServiceLocator.GetService<SurveyService>();
        var workoutService = ServiceLocator.GetService<WorkoutService>();
        UserIntendsState userIntendsState = ServiceLocator.GetService<UserIntendsState>();
       
        MessagesController messagesBaseController = new MessagesController(botClient, userService,userIntendsState);
        CallbackQueryBaseController callbackQueryBaseController = new CallbackQueryBaseController(botClient, userService, userIntendsState, surveyService,  workoutService);
        
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                   var message = update.Message;
                   
                    switch (message.Type)
                    {
                        case MessageType.Text:
                        {
                            await messagesBaseController.ListenRoutes(message.Text.Replace("/",""), update);
                            return;
                        }
                        case MessageType.Document:
                        {
                            await messagesBaseController.ListenRoutes(message.Document.MimeType.Replace("/",""), update);
                            return;
                        }
                        case MessageType.Contact:
                        {
                            string phoneNumber = update.Message.Contact.PhoneNumber;
                            await messagesBaseController.ListenRoutes($"phone_{phoneNumber}".Replace("/",""), update);
                            return;
                        }
                    }
                    
                    return;
                }
                case UpdateType.CallbackQuery:
                {
                    var message = update.CallbackQuery;
                    
                    callbackQueryBaseController.ListenRoutes(message.Data.Replace("/",""), update);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}