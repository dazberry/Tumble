using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tumble.Core
{
    public class PipelineHandlerCollection
    {
        private IList<IPipelineHandler> _handlers = new List<IPipelineHandler>();

        public PipelineHandlerCollection Add(IPipelineHandler handler) =>
            AddRange(handler);

        public PipelineHandlerCollection Add<T>(Action<T> handlerAction = null)
            where T : IPipelineHandler, new()
        {
            var handler = new T();
            _handlers.Add(handler);
            handlerAction?.Invoke(handler);            
            return this;
        }
        public PipelineHandlerCollection AddRange(params IPipelineHandler[] handlers)
        {
            foreach (var handler in handlers)
                _handlers.Add(handler);
            return this;
        }

        public int Count() => _handlers.Count();
        public IPipelineHandler this[int index] => _handlers[index];

        public IPipelineHandler Get<T>()
            where T : IPipelineHandler =>
            _handlers.Where(x => x is T).FirstOrDefault();                   
    }

    
}
