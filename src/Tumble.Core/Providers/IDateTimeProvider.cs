using System;

namespace Tumble.Core.Providers
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow();
    }
}
