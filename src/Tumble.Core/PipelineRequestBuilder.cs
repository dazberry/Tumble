using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Core
{
   
    public class PipelineRequestBuilder
    {       

        private class NamedPipelineHandler
        {
            public string Name { get; set; }
            public IPipelineHandler PipelineHandler { get; set; }
        }

        private readonly PipelineHandlerCollection _pipelineRequestCollection;
                       
        private readonly PipelineRequest _pipelineRequest;
        public PipelineRequest PipelineRequest => _pipelineRequest;

        private readonly PipelineContext _pipelineContext;
        public PipelineContext PipelineContext => _pipelineContext;
                      
        public PipelineRequestBuilder(PipelineHandlerCollection pipelineHandlerCollection) 
            : this(new PipelineContext(), pipelineHandlerCollection) { }
        
        public PipelineRequestBuilder(PipelineContext pipelineContext, PipelineHandlerCollection pipelineHandlerCollection)
        {
            _pipelineRequestCollection = pipelineHandlerCollection;
            _pipelineContext = pipelineContext;
            _pipelineRequest = new PipelineRequest();
        }

        public PipelineRequestBuilder AddHandler<T>(Action<T, PipelineContext> addHandlerAction = null)
            where  T : IPipelineHandler, new()
        {
            var handler = (T)_pipelineRequestCollection.Get<T>();
            if (handler == null) handler = new T();
            addHandlerAction?.Invoke(handler, _pipelineContext);
            _pipelineRequest.AddHandler(handler);
            return this;
        }
               
        public PipelineRequestBuilder AddHandler<T>(T handler, Action<T, PipelineContext> addHandlerAction = null)
            where T : IPipelineHandler
        {                        
            addHandlerAction?.Invoke(handler, _pipelineContext);
            _pipelineRequest.AddHandler(handler);            
            return this;
        }

        public PipelineRequestBuilder WithContext(Action<PipelineContext> contextAction)
        {
            contextAction?.Invoke(_pipelineContext);
            return this;
        }
       
        public async Task<PipelineContext> InvokeAsync(Action<PipelineContext> invokeAction = null) =>
            await _pipelineRequest.InvokeAsync(_pipelineContext, invokeAction);


    }
}
