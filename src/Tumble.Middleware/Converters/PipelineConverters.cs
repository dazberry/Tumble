using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProxyMiddleware.Extensions;

namespace Tumble.Converters
{
    public static class PipelineConverters
    {
      
        public static async Task Convert(HttpResponseMessage source, HttpResponse dest, params string[] removeHeaders)
        {
            dest.StatusCode = (int)source.StatusCode;
            foreach (var header in source.Headers)
            {
                dest.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in source.Content.Headers)
            {
                dest.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var removeHeader in removeHeaders)
                dest.Headers.Remove(removeHeader);

            await source.Content.CopyToAsync(dest.Body);
        }


        public static void Convert(HttpRequest source, out HttpRequestMessage dest, params string[] removeHeaders)
        {
            dest = new HttpRequestMessage()
            {
                Method = new HttpMethod(source.Method),
                RequestUri = source.ToUri(),
            };

            if (source.Body != null)
            {
                var streamContent = new StreamContent(source.Body);
                dest.Content = streamContent;
            }

            var headerKeys =source.Headers.Select(x => x.Key).Except(removeHeaders);
            foreach (var header in source.Headers.Where(x => headerKeys.Contains(x.Key)))
            {
                if (!dest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) &&
                    dest.Content != null)
                    dest.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
            
        }
    }
}
