using System;

namespace Tumble.Core.Helpers
{
    public class LockableObject<T>
          where T : class
    {
        private object _lock = new object();

        private T _object;
        public T Object
        {
            get { lock (_lock) { return _object; } }
            set { lock (_lock) { _object = value; } }
        }

        public void Invoke(Action<T> action)
        {
            lock (_lock)
                action.Invoke(Object);
        }
    }
}
