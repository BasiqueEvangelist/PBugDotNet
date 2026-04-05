using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PBug.Utils;

/// <summary>
/// Crutch to configure Options with dependencies from a ServiceProvider
/// </summary>
public static class PostConfigureOptionsWithDi
{
    public static IServiceCollection PostConfigureWithDi<TOptions, TService>(
        this IServiceCollection serviceCollection, 
        Action<TOptions,TService> configure) 
        where TOptions : class 
        where TService : class
    {
        return serviceCollection.AddSingleton<IPostConfigureOptions<TOptions>>(
            (services) =>
            new Helper<TOptions, TService>(
                services.GetRequiredService<TService>(), 
                configure)
        );
    }

    class Helper<TOptions, TService> : IPostConfigureOptions<TOptions>
        where TOptions : class
        where TService : class
    {
        TService _Service;
        Action<TOptions, TService> _PostConfigure;

        public Helper(TService service, Action<TOptions, TService> postConfigure)
        {
            _Service = service;
            _PostConfigure = postConfigure;
        }

        public void PostConfigure(string name, TOptions options) 
            => _PostConfigure(options, _Service);
    }
}
