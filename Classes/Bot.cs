using MyBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace MyBot.Classes;

public class Bot : IBotService
{
    private static ITelegramBotClient _botClient;
    private static ReceiverOptions _receiverOptions;

    public Bot(string token, ReceiverOptions receiverOptions)
    {
        _botClient = new TelegramBotClient(token);//Environment.GetEnvironmentVariable("TelegramBotToken");
        _receiverOptions = receiverOptions;
    }

    public async Task StartAsync()
    {
        using var cts = new CancellationTokenSource();
        
         _botClient.StartReceiving(BotHandler.UpdateHandler, BotHandler.ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота
        
        var myBot = await _botClient.GetMe(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        Console.WriteLine($"{myBot.FirstName} запущен!");
        Console.ReadKey();
    }
}