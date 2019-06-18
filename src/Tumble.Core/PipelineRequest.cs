using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tumble.Core
{
    public class PipelineRequest
    {              
        private List<PipelineHandlerInfo> _pipelineHandlers = new List<PipelineHandlerInfo>();

        public PipelineRequest AddHandler<THandler>(Action<THandler> handlerAction = null)
            where THandler : class, new()
        {
            if (typeof(THandler) == typeof(PipelineHandlerInfo) ||
                typeof(THandler) == typeof(PipelineRequest))
                throw new ArgumentException($"Unsuppored Handler Type: {typeof(THandler)}");

            var pipelineHandler = new THandler();

            handlerAction?.Invoke(pipelineHandler);

            _pipelineHandlers.Add(
                new PipelineHandlerInfo()
                    .SetHandler(pipelineHandler));

            return this;
        }

        public PipelineRequest AddHandler<THandler>(THandler pipelineHandler)
            where THandler : class, new()
        {
            if (pipelineHandler == null)
                throw new ArgumentNullException(nameof(pipelineHandler));

            if (pipelineHandler is PipelineHandlerInfo)
                return AddHandler(pipelineHandler as PipelineHandlerInfo);

            if (pipelineHandler is PipelineRequest)
                return AddHandler(pipelineHandler as PipelineRequest);                
            
            _pipelineHandlers.Add(
                new PipelineHandlerInfo()
                    .SetHandler(pipelineHandler));            

            return this;
        }

        public PipelineRequest AddHandler(PipelineRequest pipelineRequest)
        {
            _pipelineHandlers.AddRange((pipelineRequest as PipelineRequest)._pipelineHandlers);
            return this;
        }

        public PipelineRequest AddHandler(PipelineHandlerInfo pipelineHandlerInfo)
        {            
            _pipelineHandlers.Add(pipelineHandlerInfo);
            return this;
        }

        public PipelineRequest AddHandlers(params PipelineHandlerInfo[] pipelineHandlerInfos)
        {
            _pipelineHandlers.AddRange(pipelineHandlerInfos);
            return this;
        }



        //public PipelineRequest AddHandler<THandlerContext, TRequestContext>(
        //    IPipelineHandler<TRequestContext> pipelineHandler,
        //    Func<TRequestContext, THandlerContext> contextAction)
        //{
        //    if (pipelineHandler == null)
        //        throw new ArgumentNullException(nameof(pipelineHandler));

        //    _pipelineHandlers.Add(
        //        new PipelineHandlerInfo()
        //            .SetHandler(pipelineHandler)
        //            .SetContextAction(contextAction));

        //    return this;
        //}

        //public PipelineRequest AddHandler<THandlerContext>(
        //    Action<IPipelineHandler<THandlerContext>> handlerAction = null)
        //    where THandlerContext : IPipelineHandler<THandlerContext>, 
        //    new()
        //{
        //    var handler = new THandlerContext();
        //    handlerAction?.Invoke(handler);            
        //    _pipelineHandlers.Add(
        //        new PipelineHandlerInfo()
        //            .SetHandler(handler));
        //    return this;
        //}

#pragma warning disable IDE0039
        public async Task<TInvokeContext> InvokeAsync<TInvokeContext>(
            TInvokeContext invokeContext, 
            Action<TInvokeContext> invokeAction = null)
        {           
            if (_pipelineHandlers
                    .Where(x => x.HasContextAction())
                    .Any(x => !x.CanInvokeWithContext(invokeContext))
                )
                throw new Exception("Can not invoke handler with context");

            int index = 0;
            
            PipelineDelegate pipelineDelegate = null;
            pipelineDelegate =
                async () =>
                {
                    var nextHandler = _pipelineHandlers.Skip(index).FirstOrDefault();
                    if (nextHandler == null)
                        return;

                    index += 1;

                    var ctx = nextHandler.HasContextAction()
                        ? nextHandler.InvokeContextAction(invokeContext, out object pipelineContext)
                            ? pipelineContext
                            : invokeContext
                        : invokeContext;

                    await nextHandler.InvokePipelineHandler(pipelineDelegate, ctx);
                };
#pragma warning restore IDE0039

            try
            {
                invokeAction?.Invoke(invokeContext);
                await pipelineDelegate();
            }
            catch (Exception ex)
            {
                throw;
                //var handler = _pipelineHandlers.Skip(index - 1).FirstOrDefault();
                //ctx
                //    .AddNotification(handler, $"Unhandled Exception: {ex.Message}")
                //    .AddNotification(handler, ex.ToString());
            }

            return invokeContext;
        }

    }
}
