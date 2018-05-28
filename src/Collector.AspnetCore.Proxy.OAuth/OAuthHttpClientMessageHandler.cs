using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Collector.AspnetCore.Proxy.OAuth
{
    public class OAuthHttpClientMessageHandler : HttpClientHandler
    {
        private readonly ITokenRetriver _tokenRetriver;

        public OAuthHttpClientMessageHandler(ITokenRetriver tokenRetriver)
        {
            _tokenRetriver = tokenRetriver ?? throw new ArgumentNullException(nameof(tokenRetriver));
            AllowAutoRedirect = false;
            //UseCookies = false;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenRetriver.GetToken();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            token = await _tokenRetriver.GetToken(true);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}