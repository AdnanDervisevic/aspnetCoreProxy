using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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
                        {
                            logger.LogInformation("Client initiated {RequestMethod} request {@RequestUrl}", context.Request.Method, context.Request.Path.Value);
                            context.Request.EnableRewind(); // If we experience slowdown, this should be culprit
                        }

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
            {
                LogRequestBody(logger, context);
                logger.LogWarning("Unsuccesful response {@Response}, Body: {@Info}", new { res.Content.Headers.ContentType.MediaType, res.RequestMessage.RequestUri }, await res.Content.ReadAsStringAsync());
            }

            await res.Content.CopyToAsync(context.Response.Body);
        }
        
        private static void LogRequestBody(ILogger logger, HttpContext context)
        {
            string body;

            context.Request.Body.Position = 0;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                body = reader.ReadToEnd();
            }

            logger.LogInformation("Request Body: {@Request}", body);
        }

        private string GetRequestUri(HttpRequest request)
        {
            return request.Path.Value.Substring(_substringLength);
        }
    }
}