using System.Net;
using System.Net.Http;

namespace Tumble.Client.Extensions
{
    public static class HttpResponseExtensions
    {
        private static bool IsTransientFailure(this HttpStatusCode httpStatusCode) =>
            httpStatusCode == 0 || 
            (int)httpStatusCode == 429 || 
            (int)httpStatusCode >= 500;

        public static bool IsTransientFailure(this HttpResponseMessage httpResponseMessage) =>
            httpResponseMessage.StatusCode.IsTransientFailure();            
    }
}
