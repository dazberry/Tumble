using System;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Tumble.Client.Extensions
{
    public static class UriExtensions
    {
        public static string ToUriString(this HttpRequest httpRequest) =>
            $"{httpRequest.Scheme}{Uri.SchemeDelimiter}{httpRequest.Host.Host}:{httpRequest.Host.Port}{httpRequest.PathBase}{httpRequest.Path}{httpRequest.QueryString}";
       
        public static Uri ToUri(this HttpRequest httpRequest) =>        
            new Uri(httpRequest.ToUriString());

        public static Uri Append(this Uri uri, string route)
        {
            //var segments = uri.Segments.Select(x => x.TrimEnd('/')).Where(x => !String.IsNullOrEmpty(x));
            //var newSegments = route.Split('/').Select(x => x.TrimEnd('/'));
            //segments = segments.Concat(newSegments).Where(x => !string.IsNullOrEmpty(x));
            //var result = string.Join('/', segments);

            var segments = uri.Segments.Concat(route.Split('/'))
                    .Where(x => !string.IsNullOrEmpty(x) && x != "/")
                    .Select(x => x.Trim('/'))
                    .ToArray();

            return new Uri($"{uri.Scheme}{Uri.SchemeDelimiter}{uri.Host}{(uri.Port == 0 ? "" : $":{uri.Port}")}/{string.Join('/', segments)}{uri.Query}");                      
        }

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
