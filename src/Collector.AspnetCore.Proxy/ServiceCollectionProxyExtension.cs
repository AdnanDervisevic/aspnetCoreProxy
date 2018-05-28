using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Collector.AspnetCore.Proxy
{
    public static class ServiceCollectionProxyExtension
    {

        public static IServiceCollection AddProxyClient<TOptions>(this IServiceCollection services, TOptions options) where TOptions : class, IProxyOptions, new()
        {
            return services.AddSingleton(provider => new InternalProxyMiddlewareOption<TOptions>(options))
             .AddTransient<IProxyClient>(provider => new TypedDefaultProxyProxyClient<TOptions>(options));
        }

        public static IServiceCollection AddProxyClient<TOptions>(this IServiceCollection services, TOptions options, HttpClientHandler httpClientHandler) where TOptions : class, IProxyOptions, new()
        {
            return services.AddSingleton(provider => new InternalProxyMiddlewareOption<TOptions>(options))
                .AddTransient<IProxyClient>(provider => new TypedDefaultProxyProxyClient<TOptions>(options, httpClientHandler));
        }

        public static IServiceCollection AddProxyClient<TOptions>(this IServiceCollection services) where TOptions : class, IProxyOptions, new()
        {
            return services.AddSingleton(provider => new InternalProxyMiddlewareOption<TOptions>(provider.GetRequiredService<IOptions<TOptions>>().Value)).AddTransient<IProxyClient>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<TOptions>>().Value;
                return new TypedDefaultProxyProxyClient<TOptions>(options);
            });
        }

        public static IServiceCollection AddProxyClient<TOptions>(this IServiceCollection services, HttpClientHandler httpClientHandler) where TOptions : class, IProxyOptions, new()
        {
            return services.AddSingleton(provider => new InternalProxyMiddlewareOption<TOptions>(provider.GetRequiredService<IOptions<TOptions>>().Value))
                .AddTransient<IProxyClient>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<TOptions>>().Value;
                return new TypedDefaultProxyProxyClient<TOptions>(options, httpClientHandler);
            });
        }
    }
}
