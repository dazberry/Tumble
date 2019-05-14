using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Security.Contexts
{
    public interface IResponseSanitizerContext : IHttpRequestResponseContext
    {   
        Guid? ErrorReference { get; set; }
        HttpResponseMessage OriginalResponse { get; set; }
    }
}
