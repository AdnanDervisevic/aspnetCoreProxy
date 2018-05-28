using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Collector.AspnetCore.Proxy
{
    public class DefaultProxyProxyClient:IProxyClient
    {
        private readonly HttpClient _httpClient;

        public DefaultProxyProxyClient(IProxyOptions proxyOptions, HttpClientHandler httpClientHandler)
        {
            if (proxyOptions == null) throw new ArgumentNullException(nameof(proxyOptions));
            if (httpClientHandler == null) throw new ArgumentNullException(nameof(httpClientHandler));

            _httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(proxyOptions.BaseUrl)
            };
        }

        public DefaultProxyProxyClient(IProxyOptions proxyOptions)
        {
            if (proxyOptions == null) throw new ArgumentNullException(nameof(proxyOptions));
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(proxyOptions.BaseUrl)
            };
        }
        public Task<HttpResponseMessage> SendAsync(string requestUri, HttpRequest request, CancellationToken cancellationToken = default) =>
            _httpClient.SendAsync(CreateHttpRequestMessage(request, requestUri));

        private static HttpRequestMessage CreateHttpRequestMessage(HttpRequest request, string requestUri)
        {
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(request.Method), requestUri);

            var requestMethod = request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                httpRequestMessage.Content = new StreamContent(request.Body);
            }


            foreach (var header in request.Headers)
            {
                if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    httpRequestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            return httpRequestMessage;
        }
       
    }
}