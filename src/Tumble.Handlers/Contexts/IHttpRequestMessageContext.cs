using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Tumble.Handlers.Contexts
{
    public interface IHttpRequestMessageContext
    {
        HttpRequestMessage HttpRequestMessage { get; set; }
    }
}
