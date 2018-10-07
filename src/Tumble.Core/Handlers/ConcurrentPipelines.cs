using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Core.Handlers
{
    public class ConcurrentPipelines : IPipelineHandler
    {
        private class RequestAndContext
        {
            public PipelineRequest Request { get; set; }
            public PipelineContext Context { get; set; }
        }

        private List<RequestAndContext> _requestAndContexts = new List<RequestAndContext>();
       
        public ConcurrentPipelines Add(PipelineRequest request, PipelineContext context)
        {
            _requestAndContexts.Add(new RequestAndContext() { Request = request, Context = context });
            return this;
        }            

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            List<Task> tasks = new List<Task>();

            foreach (var requestAndContext in _requestAndContexts)
            {
                var task = requestAndContext.Request.InvokeAsync(requestAndContext.Context);
                context.Add(requestAndContext.Context);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());
            
            await next.Invoke();
        }
    }
}
