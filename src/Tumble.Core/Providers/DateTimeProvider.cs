using System;

namespace Tumble.Core.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow() =>
            DateTime.UtcNow;
        
    }
}
