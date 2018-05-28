using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Collector.AspnetCore.Proxy
{
    internal class ProxyMiddleware<TOptions> where TOptions : class, IProxyOptions, new()
    {
        private readonly IApplicationBuilder _app;
        private readonly InternalProxyMiddlewareOption<TOptions> _options;
        private readonly int _substringLength;

        public ProxyMiddleware(IApplicationBuilder app, InternalProxyMiddlewareOption<TOptions> options)
        {
            _app = app;
            _options = options;
            _substringLength = _options.Options.SubstringStartWith?.Length ?? 0;
        }

        
        public void Run()
        {
            _app.Run(async (context) =>
            {

                var logger = context.RequestServices.GetService<ILoggerFactory>()
                    .CreateLogger<TOptions>();

                try
                {
                    if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith(_options.Options.UrlStartsWith))
                    {
                        if (_options.Options.ChallangeAuthenticate)
                        {
                            await context.ChallengeAsync(_options.Options.ChallangeAuthenticateSchema);
                            if (!context.User.Identity.IsAuthenticated)
                                return;
                        }


                        if (_options.Options.UseLogger)
                            logger.LogDebug("Initiating request {@Request}", context.Request);

                        var client = context.RequestServices.GetService<ITypedProxyClient<TOptions>>();

                        await Execute(logger, context, () => client.SendAsync(GetRequestUri(context.Request), context.Request, context.RequestAborted));

                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while proxing request with {@client}", typeof(TOptions).Name);

                    throw;
                }

            });
        }
        private async Task Execute(ILogger logger, HttpContext context, Func<Task<HttpResponseMessage>> func)
        {
            var res = await func();
            context.Response.StatusCode = (int)res.StatusCode;

            if (_options.Options.UseLogger && !res.IsSuccessStatusCode)
                logger.LogInformation("Unsuccesful response {@Response}", res);

            await res.Content.CopyToAsync(context.Response.Body);
        }

        private string GetRequestUri(HttpRequest request)
        {
            return request.Path.Value.Substring(_substringLength);
        }
    }
}