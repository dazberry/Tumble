using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tumble.Core
{
    public class PipelineRequest
    {
        private IList<IPipelineHandler> _pipelineHandlers = new List<IPipelineHandler>();

        public PipelineRequest AddHandler(IPipelineHandler pipelineHandler)
        {
            _pipelineHandlers.Add(pipelineHandler);
            return this;
        }

        public PipelineRequest AddHandler<T>(Action<T> handlerAction = null)
            where T : IPipelineHandler, new()
        {
            var handler = new T();
            handlerAction?.Invoke(handler);
            _pipelineHandlers.Add(handler);            
            return this;
        }

        public async Task InvokeAsync(PipelineContext context)
        {
            int index = 0;
                       
            PipelineDelegate pipelineDelegate = null;
            pipelineDelegate =
                async () =>
                {                                
                    var nextHandler = _pipelineHandlers.Skip(index).FirstOrDefault();
                    index += 1;
                    if (nextHandler != null)
                        await nextHandler.InvokeAsync(context, pipelineDelegate);
                };

            try
            {       
                await pipelineDelegate.Invoke();             
            }            
            catch (Exception ex)
            {            
                var handler = _pipelineHandlers.Skip(index).FirstOrDefault();
                context.Add(new PipelineException(handler, "Unhandled exception", ex));                
            }
        }
    }
}
