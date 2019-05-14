using System;
using System.Collections.Generic;
using System.Text;
using Tumble.Client.Parameters;

namespace Tumble.Client.Contexts
{
    public interface IQueryParameterContext
    {
        IQueryParameters QueryParameters { get; set; }
    }
}
