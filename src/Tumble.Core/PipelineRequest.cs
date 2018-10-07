using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Tumble.Core
{
    public class PipelineRequest
    {
        private IList<IPipelineHandler> _pipelineHandlers = new List<IPipelineHandler>();
             
        public PipelineRequest AddHandler(IPipelineHandler pipelineHandler)
        {
            if (pipelineHandler == null)
                throw new ArgumentNullException("PipelineRequest.AddHandler. PipelineHandler can not be null");
            _pipelineHandlers.Add(pipelineHandler);
            return this;
        }

        public PipelineRequest AddHandlerFromCollection<T>(PipelineHandlerCollection handlers)
            where T : IPipelineHandler
        {
            var handler = handlers.Get<T>();
            if (handler == null)
                throw new ArgumentException("PipelineRequest.AddHandlerFromCollection. Typed handler not found in collection.");
            return AddHandler(handler);
        }

        public PipelineRequest AddHandler<T>(Action<T> handlerAction = null)
            where T : IPipelineHandler, new()
        {
            var handler = new T();
            handlerAction?.Invoke(handler);
            _pipelineHandlers.Add(handler);            
            return this;
        }        

        public async Task<PipelineContext> InvokeAsync(PipelineContext context, Action<PipelineContext> invokeAction = null)
        {
            int index = 0;
            var ctx = context ?? new PipelineContext();

            PipelineDelegate pipelineDelegate = null;
            pipelineDelegate =
                async () =>
                {
                    var nextHandler = _pipelineHandlers.Skip(index).FirstOrDefault();
                    index += 1;
                    if (nextHandler != null)
                        await nextHandler.InvokeAsync(ctx, pipelineDelegate);
                };

            try
            {
                invokeAction?.Invoke(ctx);
                await pipelineDelegate.Invoke();
            }
            catch (Exception ex)
            {
                var handler = _pipelineHandlers.Skip(index).FirstOrDefault();
                context.Add(new PipelineException(handler, "Unhandled exception", ex));
            }

            return ctx;
        }

        public async Task<PipelineContext> InvokeAsync(Action<PipelineContext> invokeAction = null) =>
            await InvokeAsync(null, invokeAction);       
    }
}
