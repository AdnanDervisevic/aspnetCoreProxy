using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Collector.AspnetCore.Proxy
{
    public interface IProxyClient
    {
        Task<HttpResponseMessage> SendAsync(string uri, HttpRequest message, CancellationToken token = default);
    }
}