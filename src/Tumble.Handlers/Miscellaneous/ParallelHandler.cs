using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Miscellaneous
{
    public interface IUnhandledHandlerException
    {
        Exception UnhandledException { get; set; }
    }

    public class ParallelHandler<TContext> : IPipelineHandler
    {                
        public class PipelineRequestAndContext
        {
            public PipelineRequest PipelineRequest { get; set; }
            public TContext Context { get; set; }
        }

        private IList<PipelineRequestAndContext> _pipelineRequestAndContext = new List<PipelineRequestAndContext>();

        public ParallelHandler<TContext> Add(PipelineRequest request, TContext context, Action<TContext> contextAction = null)
        {
            _pipelineRequestAndContext.Add(new PipelineRequestAndContext()
            {
                PipelineRequest = request,
                Context = context
            });
            contextAction?.Invoke(context);
            return this;
        }

        public IEnumerable<TContext> Contexts => _pipelineRequestAndContext.Select(x => x.Context);

        public int MaxConcurrentTasks { get; set; } = 0;
                      
        public async Task InvokeAsync(PipelineDelegate next)
        {       
            Parallel.ForEach(_pipelineRequestAndContext,
                req =>
                {
                    try
                    {
                        var task = req.PipelineRequest.InvokeAsync(req.Context);
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        if (req.Context is IUnhandledHandlerException)
                            (req.Context as IUnhandledHandlerException).UnhandledException = ex;
                    }                    
                });

           
            await next.Invoke();
        }
    }
}
