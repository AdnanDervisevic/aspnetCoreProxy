namespace Collector.AspnetCore.Proxy.OAuth
{
    public interface IOAuthProxyOptions : IProxyOptions
    {
        string Name { get; }
        string Audience { get; }
        string Scopes { get; }
    }
}