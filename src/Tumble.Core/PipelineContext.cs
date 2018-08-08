using System;
using System.Collections.Generic;
using System.Linq;

namespace Tumble.Core
{
    public class PipelineContext
    {
        public readonly Guid Id = Guid.NewGuid();

        private IList<IPipelineContextItem> _pipelineContextList = new List<IPipelineContextItem>();

        public PipelineContext Add<T>(T item)
        {
            var contextItem = new PipelineContextItem<T>(item);
            _pipelineContextList.Add(contextItem);
            return this;
        }

        public PipelineContext AddOrReplace<T>(T item, Func<IPipelineContextItem, bool> selectAction = null)
        {            
            var items = _pipelineContextList.Where(x => x.Is<T>() && (selectAction?.Invoke(x) ?? true));
            if (items.Any())
                _pipelineContextList = new List<IPipelineContextItem>(_pipelineContextList.Except(items));            
            return Add(item);
        }

        public PipelineContext Remove<T>()
        {
            var items = _pipelineContextList.Where(x => !x.Is<T>());
            _pipelineContextList = new List<IPipelineContextItem>(items);
            return this;
        }

        public PipelineContext Remove<T>(T item)
        {
            var items = _pipelineContextList.Where(x => x.Is<T>() && x.As<T>().Equals(item));
            _pipelineContextList = _pipelineContextList.Except(items).ToList();
            return this;
        }

        public PipelineContext Remove<T>(Func<T, bool> compareAction)
        {
            var items = _pipelineContextList.Where(x => x.Is<T>() && (compareAction?.Invoke(x.As<T>()) ?? false));
            _pipelineContextList = _pipelineContextList.Except(items).ToList();
            return this;
        }

        public int Count =>
            _pipelineContextList.Count();

        public IPipelineContextItem this[int index] =>
            _pipelineContextList[index];        
        
        public IEnumerable<T> Get<T>() =>
            _pipelineContextList
                .Where(x => x.Is<T>())
                .Select(x => x.As<T>());

        public bool Get<T>(out IEnumerable<T> values)
        {
            values = Get<T>();
            return values.Any();
        }

        public bool GetFirst<T>(out T value)
        {
            value = default(T);
            var items = Get<T>();
            if (items.Any())            
                value = items.First();
            return items.Any();
        }

        private class PipelineContextItem : IPipelineContextItem
        {
            public bool Is<T>() =>
                this is PipelineContextItem<T>;

            public T As<T>() =>
                Is<T>()
                    ? ((PipelineContextItem<T>)this).Entity
                    : default(T);
        }

        private class PipelineContextItem<T> : PipelineContextItem
        {
            public T Entity { get; set; }

            public PipelineContextItem(T entity)
            {
                Entity = entity;
            }
        }
    }




}
