using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelinedApi.Models;
using Tumble.Core;

namespace PipelinedApi.Handlers
{
    public interface IGenerateObjectResultContext<T>
    {
        T Response { get; set; }

        ObjectResult ObjectResult { get; set; }
    }

    public class GenerateObjectResult : IPipelineHandler<IGenerateObjectResultContext<IEnumerable<DublinBikeStation>>>
    {
        public async Task InvokeAsync(PipelineDelegate next, IGenerateObjectResultContext<IEnumerable<DublinBikeStation>> context)
        {
            try
            {
                await next.Invoke();
            }
            finally
            {
                if (context.Response != null)
                    context.ObjectResult = new ObjectResult(context.Response) { StatusCode = 200 };
                else
                {
                    context.ObjectResult = new ObjectResult(
                        new
                        {
                            //context.Id,
                            //notifications = context
                            //    .Get<Notification>()
                            //    .Select((x, i) =>
                            //    new
                            //    {
                            //        id = i + 1,
                            //        Handler = x.Handler.ToString(),
                            //        x.ErrorMessage
                            //    })
                        })
                    { StatusCode = 500 };
                }                
            }
        }
    }
}
