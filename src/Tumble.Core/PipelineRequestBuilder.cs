using System;
using System.Collections.Generic;
using System.Text;

namespace Tumble.Core
{
    public class PipelineRequestBuilder
    {       
        private readonly PipelineHandlerCollection _pipelineRequestCollection;
        private readonly PipelineContext _pipelineContext;
        private readonly PipelineRequest _pipelineRequest;
        public PipelineRequest PipelineRequest => _pipelineRequest;
               
        public PipelineRequestBuilder(PipelineHandlerCollection pipelineHandlerCollection) 
            : this(new PipelineContext(), pipelineHandlerCollection) { }
        
        public PipelineRequestBuilder(PipelineContext pipelineContext, PipelineHandlerCollection pipelineHandlerCollection)
        {
            _pipelineRequestCollection = pipelineHandlerCollection;
            _pipelineContext = pipelineContext;
            _pipelineRequest = new PipelineRequest();
        }

        public PipelineRequestBuilder AddHandler<T>()
            where T : IPipelineHandler, new()
        {
            var handler = _pipelineRequestCollection.Get<T>();
            if (handler == null)
                handler = new T();
            _pipelineRequest.AddHandler(handler);           
            return this;
        }

        public PipelineRequestBuilder AddHandler<T>(Action<T> addHandlerAction)
            where  T : IPipelineHandler, new()
        {
            var handler = new T();
            addHandlerAction?.Invoke(handler);
            _pipelineRequest.AddHandler(handler);
            return this;
        }

        public PipelineRequestBuilder AddHandler<T>(T handler, Action<T>handlerAction)
            where T : IPipelineHandler
        {                        
            handlerAction?.Invoke(handler);
            _pipelineRequest.AddHandler(handler);            
            return this;
        }
    }
}
