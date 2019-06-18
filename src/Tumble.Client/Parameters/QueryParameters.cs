using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Tumble.Client.Parameters
{
    public class QueryParameters : IQueryParameters
    {
        private IList<QueryParameter> _queryParameters = new List<QueryParameter>();

        public IQueryParameters Add<T>(string name, T value, bool optional = false)
        {
            _queryParameters.Add(
                new QueryParameter()
                {
                    Name = name,
                    Value = value?.ToString(),
                    Optional = optional
                });
            return this;
        }

        public IEnumerable<QueryParameter> Get() =>
            _queryParameters;
    }
}
