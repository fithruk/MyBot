using Microsoft.Extensions.DependencyInjection;

namespace MyBot.Service;

public static class ServiceLocator
{
    public static IServiceProvider _inctance { get; private set; }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        _inctance = serviceProvider;
    }

    public static T GetService<T>()
    {
        return _inctance.GetRequiredService<T>();
    }
}