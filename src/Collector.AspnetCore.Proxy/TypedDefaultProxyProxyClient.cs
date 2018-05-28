using System.Net.Http;

namespace Collector.AspnetCore.Proxy
{
    public class TypedDefaultProxyProxyClient<TOptions>: DefaultProxyProxyClient, ITypedProxyClient<TOptions> where TOptions : class, IProxyOptions, new()
    {
        public TypedDefaultProxyProxyClient(TOptions proxyClientOptions, HttpClientHandler httpClientHandler) : base(proxyClientOptions, httpClientHandler)
        {
            Options = proxyClientOptions;
        }

        public TypedDefaultProxyProxyClient(TOptions proxyClientOptions) : base(proxyClientOptions)
        {
            Options = proxyClientOptions;
        }

        public TOptions Options { get; }
    }
}