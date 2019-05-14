using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Tumble.Handlers.Contexts
{
    public interface IHttpResponseMessageContext
    {
        HttpResponseMessage HttpResponseMessage { get; set; }
    }
}
