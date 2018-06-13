namespace Collector.AspnetCore.Proxy
{
    public interface IProxyOptions
    {
        string BaseUrl { get; }
        bool UseLogger { get;  }
        bool ChallengeAuthenticate { get;  }
        string ChallengeAuthenticateSchema { get; }
        string UrlStartsWith { get; }
        string SubstringStartWith { get; }
    }
}