using System.Threading.Tasks;

namespace Collector.AspnetCore.Proxy.OAuth
{
    public interface ITokenRetriver<TOptions> : ITokenRetriver where TOptions : class, IOAuthProxyOptions, new()
    {

    }
    public interface ITokenRetriver

    {
        Task<string> GetToken(bool forceUpdate = false);
    }
}