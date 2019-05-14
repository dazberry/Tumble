using System.Collections.Generic;

namespace Tumble.Client.Parameters
{
    public interface IQueryParameters
    {
        IQueryParameters Add<T>(string name, T value, bool optional = false);

        IEnumerable<QueryParameter> Get();
    }
}
