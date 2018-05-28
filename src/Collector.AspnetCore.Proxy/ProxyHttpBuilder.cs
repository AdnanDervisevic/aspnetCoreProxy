using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Collector.AspnetCore.Proxy
{
    public static class ProxyApplicationBuilderExtension{
      
        public static IApplicationBuilder UseProxy<TOptions>(this IApplicationBuilder appBuilder) where TOptions : class, IProxyOptions, new()
        {
            var options = appBuilder.ApplicationServices.GetRequiredService<InternalProxyMiddlewareOption<TOptions>>();
            appBuilder.MapWhen(
                context => context.Request.Path.HasValue &&
                           context.Request.Path.Value.StartsWith(options.Options.UrlStartsWith),
                app =>
                {
                    new ProxyMiddleware<TOptions>(app, options).Run();
                });

            return appBuilder;
        }
    }
}