using System;
using System.Collections.Generic;
using System.Text;

namespace Tumble.Core
{
    public interface IPipelineHandlerContextResolver
    {
        object Resolve<THandler, TContext, TResolvedContext>(THandler handler, TContext context, int parameterIndex);
    }
}
