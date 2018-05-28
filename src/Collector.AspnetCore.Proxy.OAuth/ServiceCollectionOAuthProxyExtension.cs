using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Collector.AspnetCore.Proxy.OAuth
{
    public static class ServiceCollectionOAuthProxyExtension
    {
        public static IServiceCollection AddOauthProxyClient<TOptions>(this IServiceCollection services) where TOptions : class, IOAuthProxyOptions, new()
        {
            services.AddTransient<ITokenRetriver<TOptions>, TokenRetiver<TOptions>>();
            return services.AddTransient<IProxyClient>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<TOptions>>().Value;
                var tokenRetriver = provider.GetRequiredService<ITokenRetriver<TOptions>>();
                return new TypedDefaultProxyProxyClient<TOptions>(options, new OAuthHttpClientMessageHandler(tokenRetriver));
            });
        }
        public static IServiceCollection AddCustomerOauthProxyClient<TOptions>(this IServiceCollection services) where TOptions : class, IOAuthProxyOptions, new()
        {
            services.AddTransient<ITokenRetriver<TOptions>, CustomerTokenRetiver<TOptions>>();
            return services.AddTransient<IProxyClient>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<TOptions>>().Value;
                var tokenRetriver = provider.GetRequiredService<ITokenRetriver<TOptions>>();
                return new TypedDefaultProxyProxyClient<TOptions>(options, new OAuthHttpClientMessageHandler(tokenRetriver));
            });
        }
    }
}