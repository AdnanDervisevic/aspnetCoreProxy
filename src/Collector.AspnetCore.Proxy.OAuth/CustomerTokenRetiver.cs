using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Collector.AspnetCore.Proxy.OAuth
{
    public class CustomerTokenRetiver<TOptions> : TokenRetiver<TOptions> where TOptions : class, IOAuthProxyOptions, new()
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerTokenRetiver(IOptions<OAuthOptions> curity, IOptions<TOptions> clientOptions, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
            : base(curity, clientOptions, cache)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private LoggedInUser GetUser()
        {
            return UserClaimHelpers.GetLoggedInUser(_httpContextAccessor.HttpContext.User);
        }
        protected override IDictionary<string, string> GetFormParameters()
        {
            var user = GetUser();

            return new Dictionary<string, string>(base.GetFormParameters())
            {
                ["sub"] = user.CustomerNumber,
                ["countrycode"] = user.CountryCode.ToUpperInvariant()
            };
        }

        protected override string GetKey()
        {
            var user = GetUser();
            return $"{Options.Name}-{user.CustomerNumber}";
        }
    }
}