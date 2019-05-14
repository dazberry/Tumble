using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tumble.Core
{
    public class PipelineHandlerCollection
    {
        private IList<PipelineHandlerInfo> _pipelineHandlers = new List<PipelineHandlerInfo>();

        public PipelineHandlerCollection Add<THandler>(THandler pipelineHandler) 
            where THandler : class, new()
        {
            if (pipelineHandler == null)            
                throw new ArgumentNullException(nameof(pipelineHandler));
                       
            _pipelineHandlers.Add(
                new PipelineHandlerInfo()
                    .SetHandler(pipelineHandler));

            return this;
        }

        public PipelineHandlerCollection Add<THandler>(Action<THandler> handlerAction = null)
            where THandler : class, new()
        {
            var pipelineHandler = new THandler();            
            handlerAction?.Invoke(pipelineHandler);

            _pipelineHandlers.Add(
                new PipelineHandlerInfo()
                    .SetHandler(pipelineHandler));

            return this;
        }

        public int Count() => _pipelineHandlers.Count();

        public PipelineHandlerInfo this[int index] => _pipelineHandlers[index];

        public PipelineHandlerInfo Get<THandler>()
            where THandler : class =>
                _pipelineHandlers.Where(x => x.Is<THandler>())
                    .FirstOrDefault();                    
    }


    }
