using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Collector.AspnetCore.Proxy.OAuth
{
    public class TokenRetiver<TOptions>
        : ITokenRetriver<TOptions> where TOptions : class, IOAuthProxyOptions, new()
    {
        protected readonly IOAuthProxyOptions Options;
        private readonly IMemoryCache _cache;
        protected readonly HttpClient TokenClient = new HttpClient();
        private readonly OAuthOptions _options;
        private readonly IDictionary<string, string> _form;

        public TokenRetiver(IOptions<OAuthOptions> curity, IOptions<TOptions> clientOptions, IMemoryCache cache)
        {
            Options = clientOptions.Value;
            _cache = cache;
            _options = curity.Value;
            TokenClient.DefaultRequestHeaders.Add("Accept", "application/jwt, application/json");
            _form = new Dictionary<string, string>
            {
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["aud"] = Options.Audience,
                ["grant_type"] = "client_credentials",
                ["scope"] = Options.Scopes,
            };
        }

        private Task<HttpResponseMessage> FetchAuthToken()
        {
            var form = GetFormParameters();

            var content = new FormUrlEncodedContent(form);

            return TokenClient.PostAsync(_options.TokenEndpoint, content);
        }


       

        protected virtual string GetKey() => Options.Name;

        protected virtual IDictionary<string, string> GetFormParameters() => _form;

        public async Task<string> GetToken(bool forceUpdate = false)
        {
            if (_cache.TryGetValue(GetKey(), out string token) && !forceUpdate)
                return token;
            var response = await FetchAuthToken();

            if (!response.IsSuccessStatusCode) return null;
            var stringResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OAuthResponse>(stringResult);
            _cache.Set(GetKey(), result.AccessToken, TimeSpan.FromSeconds(result.ExpiresIn));
            return result.AccessToken;
        }
        internal class OAuthResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}