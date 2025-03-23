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
        UserIntendsState userIntendsState = ServiceLocator.GetService<UserIntendsState>();
        string[] routes = ["/start", "/inline", "/loadCalendarFile"];
        string[] callbackRoutes = ["/loadCalendarFile"];
        MessagesController MessagesController = new MessagesController(routes, botClient, userService,userIntendsState);
        CallbackQueryController callbackQueryController = new CallbackQueryController(callbackRoutes, botClient, userService, userIntendsState);
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
                            MessagesController.listenRouts(message.Text.Replace("/",""), update);
                            return;
                        }
                        case MessageType.Document:
                        {
                            MessagesController.listenRouts(message.Document.MimeType.Replace("/",""), update);
                            return;
                        }
                    }
                    
                    return;
                }
                case UpdateType.CallbackQuery:
                {
                    var message = update.CallbackQuery;
                    
                    callbackQueryController.ListenRoutes(message.Data.Replace("/",""), update);
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