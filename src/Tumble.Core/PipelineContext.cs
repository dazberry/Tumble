using System;
using System.Collections.Generic;
using System.Linq;

namespace Tumble.Core
{
    public class PipelineContext
    {
        public readonly Guid Id = Guid.NewGuid();

        private IList<IPipelineContextItem> _pipelineContextList = new List<IPipelineContextItem>();

        public PipelineContext Add<T>(T item) =>
            Add(string.Empty, item);
        
        public PipelineContext Add<T>(string name, T item)
        {
            if (string.IsNullOrEmpty(name) || Get(name) == null)
                _pipelineContextList.Add(new PipelineContextItem<T>(name, item));
            return this;
        }

        public PipelineContext AddOrReplace<T>(string name, T item)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var result = Get(name);
                if (result != null)
                    _pipelineContextList.Remove(result);
                Add(name, item);                
            }
            else
            {
                var itemsToRemove = _pipelineContextList.Where(x => !x.IsNamed && x.Is<T>());
                _pipelineContextList = _pipelineContextList.Except(itemsToRemove).ToList();                    
            }
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
            var items = _pipelineContextList.Where(x => !x.Is<T>() && string.IsNullOrEmpty(x.Name));
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

        public T Get<T>(string name)
        {
            var result = Get(name);
            if (result != null && result.Is<T>())
                return result.As<T>();
            return default(T);
        }

        public IPipelineContextItem Get(string name) =>
            _pipelineContextList
                .FirstOrDefault(x => x.Name == name);

        public bool Get<T>(string name, out T value)
        {
            value = default(T);
            var result = Get(name);            
            if (result?.Is<T>() ?? false)
            {
                value = result.As<T>();
                return true;
            }
            return false;
        }            

        public bool GetFirst<T>(out T value)
        {
            value = default(T);
            var items = Get<T>();
            if (items.Any())
            {
                value = items.First();
                return true;
            }
            return false;
        }        

        private class PipelineContextItem : IPipelineContextItem
        {
            public string Name { get; protected set; }
            public bool IsNamed => !string.IsNullOrEmpty(Name);

            public bool Is<T>() =>
                this is PipelineContextItem<T>;

            public T As<T>() =>
                Is<T>()
                    ? ((PipelineContextItem<T>)this).Entity
                    : default(T);
        }

        private class PipelineContextItem<T> : PipelineContextItem
        {            
            public T Entity { get; private set; }            

            public PipelineContextItem(T entity)
            {
                Entity = entity;
            }

            public PipelineContextItem(string name, T entity)
            {
                Name = name;
                Entity = entity;
            }
        }
    }




}
