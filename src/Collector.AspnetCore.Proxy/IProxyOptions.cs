namespace Collector.AspnetCore.Proxy
{
    public interface IProxyOptions
    {
        string BaseUrl { get; }
        bool UseLogger { get;  }
        bool ChallangeAuthenticate { get;  }
        string ChallangeAuthenticateSchema { get; }
        string UrlStartsWith { get; }
        string SubstringStartWith { get; }
    }
}