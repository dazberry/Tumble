using System;
using System.Net.Http;

namespace Tumble.Handlers.Security.Contexts
{
    public interface IResponseSanitizerContext
    {   
        Guid? ErrorReference { get; set; }
        HttpResponseMessage OriginalResponse { get; set; }
    }
}
