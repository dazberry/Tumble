using System;

namespace Tumble.Core.Extensions
{
    public static class FluentExtensions
    {
        public static T If<T>(this T value, bool conditional, Action<T> valueAction)
        {
            if (conditional)
                valueAction?.Invoke(value);
            return value;
        }

    }
}
