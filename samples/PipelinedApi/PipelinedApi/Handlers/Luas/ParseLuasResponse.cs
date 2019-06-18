using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tumble.Core;
using Tumble.Core.Contexts;

namespace PipelinedApi.Handlers.Luas
{
    /// <summary>
    /// Requires HttpResponseMessage, IContextWriter&lt;TResponse&gt;
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class ParseLuasResponse<TResponse> : IPipelineHandler<HttpResponseMessage, IContextResolver<TResponse>>
    {
        private async Task<bool> IsExceptionResponse(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms, 10);
                stream.Seek(0, SeekOrigin.Begin);

                using (var sr = new StreamReader(ms))
                {
                    var buffer = new char[10];
                    ms.Seek(0, SeekOrigin.Begin);
                    var res = sr.Read(buffer);
                    return string.Compare("Exception:", new string(buffer), true) == 0;
                }
            }
        }

        public async Task InvokeAsync(PipelineDelegate next, HttpResponseMessage httpResponseMessage, IContextResolver<TResponse> apiResponse)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                if (!await IsExceptionResponse(responseStream))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TResponse));
                    var response = serializer.Deserialize(responseStream);
                    apiResponse.Set((TResponse)response);
                }
                else
                {
                    httpResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    apiResponse.Set(default(TResponse));
                }
            }

            await next.Invoke();
        }
    }
}
