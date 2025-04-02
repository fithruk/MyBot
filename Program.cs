using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using MyBot.Service;
using MyBot.Classes;
using MyBot.Controllers;
using MyBot.MyBotUsersDBJson;
using MyBot.Repository;
using MyBot.State;
using Telegram.Bot;

namespace MyBot;

public class Program
{
    public static async Task Main(string[] args)
    {
        string botToken = "7413519615:AAH5_-yp0ir_WeQPWkLG7gzZmN5N6Ds3-wQ";
        
        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
                UpdateType.CallbackQuery,
            },
        };
       
        var serviceProvider = new ServiceCollection()
            .AddSingleton<string>(botToken)
            .AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken))
            .AddSingleton<ReceiverOptions>(receiverOptions)
            .AddSingleton<IBotService, Bot>()
            .AddSingleton<TempUsersDB>()
            .AddSingleton<UserIntendsState>()
            .AddScoped<MessagesController>()
            .AddScoped<CallbackQueryBaseController>()
            .AddScoped<UserRepository>()
            .AddScoped<UserService>()
            .BuildServiceProvider();
        
        ServiceLocator.Initialize(serviceProvider);
        
        var botService = serviceProvider.GetRequiredService<IBotService>();
        
        try
        {
            await botService.StartAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}

