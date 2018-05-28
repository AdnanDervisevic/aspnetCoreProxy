using Collector.AspnetCore.Proxy.OAuth;

namespace Collector.AspnetCore.Proxy.Sample.Configuration
{
    public class TestProxyOptions :  IOAuthProxyOptions
    {
        public string BaseUrl { get; set; }
        public bool UseLogger { get; set; }
        public bool ChallangeAuthenticate { get; set; }
        public string ChallangeAuthenticateSchema { get; set; }
        public string UrlStartsWith { get; set; }
        public string SubstringStartWith { get; set; }
        public string Name { get; set; }
        public string Audience { get; set; }
        public string Scopes { get; set; }
    }
}
