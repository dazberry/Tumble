using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Handlers.Caching.Contexts;

namespace Tumble.Handlers.Caching
{
    public enum EtagCacheEnum { NotModified }

    public class EtagCache : IPipelineHandler<IEtagCachingContext>
    {
        public async Task InvokeAsync(PipelineDelegate next, IEtagCachingContext context)
        {
            var req = context.HttpRequestMessage;
            if (req.Method != HttpMethod.Get)
            {
                await next();
                return;
            }

            var ifNoneMatch = req.Headers.IfNoneMatch;
            var eTag = ifNoneMatch?.FirstOrDefault();

            await next();

            var resp = context.HttpResponseMessage;
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
                    context.HttpResponseMessage = new HttpResponseMessage()
                        { StatusCode = System.Net.HttpStatusCode.NotModified };
                }
            }
        }
    }
    
}
