using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Tumble.Core;

namespace Tumble.Client.Contexts
{
    public interface IHttpClientContext : 
        IUriContext, 
        IQueryParameterContext
    {           
        HttpRequestMessage HttpRequestMessage { get; set; }
        HttpResponseMessage HttpResponseMessage { get; set; }

        CancellationToken CancellationToken { get; set; }
    }
}
