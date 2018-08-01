using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Caching
{
    public enum EtagCacheEnum { NotModified }

    public class EtagCache : IPipelineHandler
    {
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (!context.GetFirst(out HttpRequestMessage httpRequestMessage) ||
                httpRequestMessage.Method != HttpMethod.Get)
            {
                await next();
                return;
            }
            
            var ifNoneMatch = httpRequestMessage.Headers.IfNoneMatch;
            var eTag = ifNoneMatch?.FirstOrDefault();

            await next();

            if (context.GetFirst(out HttpResponseMessage httpResponseMessage)
                && httpResponseMessage.IsSuccessStatusCode)
            {
                string hash;
                using (var md5 = MD5.Create())
                {
                    var md5Hash = md5.ComputeHash(await httpResponseMessage.Content.ReadAsStreamAsync());
                    hash = Encoding.ASCII.GetString(md5Hash);
                    httpResponseMessage.Headers.TryAddWithoutValidation("Etag", hash);
                }

                if ((eTag != null) && (string.Compare(eTag.Tag, hash) == 0))
                {
                    context
                        .AddOrReplace(new HttpResponseMessage()
                            { StatusCode = System.Net.HttpStatusCode.NotModified })
                        .Add(EtagCacheEnum.NotModified);                    
                }                
            }            
        }
    }
}
