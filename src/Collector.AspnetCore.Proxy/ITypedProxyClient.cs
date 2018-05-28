namespace Collector.AspnetCore.Proxy
{
    public interface ITypedProxyClient<out TOption>:IProxyClient where TOption : class, IProxyOptions, new()
    {
        TOption Options { get; }
    }
}
