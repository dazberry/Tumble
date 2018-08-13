using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Core.Handlers
{

    public class ParallelPipelineInfo
    {
        public PipelineContext Context { get; set; }
        public Task ParallelTask { get; set; }

    }

    public class ParallelPipeline : IPipelineHandler
    {
        public PipelineRequest ParallelPipelineRequest { get; set; }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            PipelineContext parallelContext = new PipelineContext();
            Task parallelTask = new Task(async () => 
                await ParallelPipelineRequest.InvokeAsync(parallelContext));
            parallelTask.Start();

            context.Add(new ParallelPipelineInfo()
            {
                Context = parallelContext,
                ParallelTask = parallelTask
            });

            await Task.WhenAll(new Task[] { next.Invoke(), parallelTask });       
        }
    }
}
