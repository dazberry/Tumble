using System;
using System.Collections.Generic;
using System.Text;

namespace Tumble.Core.Contexts
{
    public interface IContextResolver<T>
    {
        T Get();
        void Set(T value);
    }

    public class ContextResolver<T> : IContextResolver<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        public ContextResolver(Func<T> getter, Action<T> setting)
        {
            _getter = getter;
            _setter = setting;
        }

        public T Get() =>
            _getter.Invoke();

        public void Set(T value) =>
            _setter(value);
    }
}
