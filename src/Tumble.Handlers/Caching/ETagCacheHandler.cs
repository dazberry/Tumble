using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Caching.Contexts;
using Tumble.Handlers.Contexts;

namespace Tumble.Handlers.Caching
{
    public enum EtagCacheEnum { NotModified }

    public class EtagCache : IPipelineHandler<IEtagCachingContext, HttpRequestMessage, IHttpResponseMessageContext>
    {
        public async Task InvokeAsync(PipelineDelegate next, IEtagCachingContext context, HttpRequestMessage httpRequestMessage, IHttpResponseMessageContext httpResponseMessage)
        {            
            if (httpRequestMessage.Method != HttpMethod.Get)
            {
                await next();
                return;
            }

            var ifNoneMatch = httpRequestMessage.Headers.IfNoneMatch;
            var eTag = ifNoneMatch?.FirstOrDefault();

            await next();

            var resp = httpResponseMessage.HttpResponseMessage;
            if (resp?.IsSuccessStatusCode ?? false)
            {
                string hash;
                using (var md5 = MD5.Create())
                {
                    var md5Hash = md5.ComputeHash(await resp.Content.ReadAsStreamAsync());
                    hash = Encoding.ASCII.GetString(md5Hash);
                    resp.Headers.TryAddWithoutValidation("Etag", hash);
                }

                if ((eTag != null) && (string.Compare(eTag.Tag, hash) == 0))
                {
                    context.FromCache = true;
                    httpResponseMessage.HttpResponseMessage = new HttpResponseMessage()
                        { StatusCode = System.Net.HttpStatusCode.NotModified };
                }
            }
        }
    }
    
}
