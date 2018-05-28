using System;

namespace Collector.AspnetCore.Proxy
{
    internal class InternalProxyMiddlewareOption<TOptions> where TOptions: IProxyOptions
    {
        public readonly IProxyOptions Options;

        public InternalProxyMiddlewareOption(TOptions options)
        {
            Options = options;
        }

    }
}