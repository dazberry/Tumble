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

            public Task InvokeAsync()
            {
                var task = Task.Run(() => Request.InvokeAsync(Context).Wait());
                return task;
            }
        }

        private List<RequestAndContext> _requestAndPipeline = new List<RequestAndContext>();

        public int ConcurrentPipelineCount { get; set; } = 5;
       
        public ConcurrentPipelines Add(PipelineRequest request, PipelineContext context)
        {
            _requestAndPipeline.Add(new RequestAndContext() { Request = request, Context = context });
            return this;
        }            

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            var concurrentPipelineCount = ConcurrentPipelineCount < 1
                    ? 1 : ConcurrentPipelineCount;

            foreach (var requestAndPipeline in _requestAndPipeline)
                context.Add(requestAndPipeline.Context);

            var remainingItems = _requestAndPipeline
                                    .Skip(concurrentPipelineCount)
                                    .Select(x => x);

            var runningTasks = _requestAndPipeline
                                    .Take(concurrentPipelineCount)
                                    .Select(x => x.InvokeAsync())
                                    .ToList();
                        
            while (runningTasks.Any())
            {
                var task = await Task.WhenAny(runningTasks.ToArray());
                runningTasks.Remove(task);

                var nextTask = remainingItems.FirstOrDefault();
                if (nextTask != null)
                {
                    runningTasks.Add(nextTask.InvokeAsync());
                    remainingItems = remainingItems.Skip(1);
                }
            }
            
            await next.Invoke();
        }
    }
}
