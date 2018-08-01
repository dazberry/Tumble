using System;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace ProxyMiddleware.Extensions
{
    public static class UriExtensions
    {
        public static string ToUriString(this HttpRequest httpRequest) =>
            $"{httpRequest.Scheme}://{httpRequest.Host.Host}:{httpRequest.Host.Port}{httpRequest.PathBase}{httpRequest.Path}{httpRequest.QueryString}";

        public static Uri ToUri(this HttpRequest httpRequest) =>        
            new Uri(httpRequest.ToUriString());  
        
        public static Uri RemoveQueryKey(this Uri uri, string key)
        {
            var queryString = HttpUtility.ParseQueryString(uri.Query);
                      
            if (queryString.AllKeys.Any(x => string.Compare(key, x, true) == 0))
            {
                queryString.Remove(key);
                var path = uri.GetLeftPart(UriPartial.Path);
                return queryString.Count > 0
                    ? new Uri($"{path}?{queryString}")
                    : new Uri(path);
            }
            return uri;
        }
    }
}
