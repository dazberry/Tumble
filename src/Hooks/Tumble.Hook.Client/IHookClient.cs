using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Hook.Client
{
    public interface IHookClient
    {
        Task<bool> QueueAsync(HttpRequestMessage httpRequestMessage);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
    }
}
