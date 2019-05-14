using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Core
{
    public class PipelineHandlerInfo
    {
        public string Name => _pipelineHandler.GetType().ToString();
        private object _pipelineHandler;
        private MethodInfo _pipelineInvokeMethodInfo;

        private MethodInfo _contextActionMethodInfo;
        private object _contextActionObject;

        public bool Is<T>() =>
            _pipelineHandler is T;

        public PipelineHandlerInfo SetHandler<THandler>(
            THandler pipelineHandler)
            where THandler : class
        {            
            _pipelineHandler = pipelineHandler
                ?? throw new ArgumentNullException(nameof(pipelineHandler));

            _pipelineInvokeMethodInfo = pipelineHandler.GetType().GetMethod("InvokeAsync");
            if (_pipelineInvokeMethodInfo == null)
                throw new Exception("");
            return this;
        }

        public static bool IsValidHandler<THandler>(THandler handler)
        {
            var invokeMethodInfo = handler?.GetType().GetMethod("InvokeAsync");
            if (invokeMethodInfo != null)
            {
                var parameters = invokeMethodInfo.GetParameters();

                return (parameters.Count() >= 2) &&
                    (parameters[0].ParameterType == typeof(PipelineDelegate));
            }
            return false;
        }
        
        public PipelineHandlerInfo SetContextAction<THandlerContext, TRequestContext>(
            Func<TRequestContext, THandlerContext> contextAction)
        {
            _contextActionMethodInfo = contextAction.Method;
            _contextActionObject = contextAction.Target;

            return this;
        }

        public bool CanInvokeWithContext<T>(T context) =>
            _pipelineInvokeMethodInfo.GetParameters().Skip(1).All(x =>
                x.ParameterType.IsAssignableFrom(context.GetType()));

        public Task InvokePipelineHandler<T>(PipelineDelegate pipelineDelegate, T context)
        {
            var parameterCount = _pipelineInvokeMethodInfo.GetParameters().Count();
            var parameters =
                new object[] { pipelineDelegate }
                .Concat(
                    Enumerable.Range(1,  parameterCount - 1)
                    .Select(x => (object)context)
                ).ToArray();

            return (Task)_pipelineInvokeMethodInfo.Invoke(_pipelineHandler, parameters);

            //(Task)_pipelineInvokeMethodInfo.Invoke(_pipelineHandler, new object[] { pipelineDelegate, context });
        }

        public bool HasContextAction() =>
            _contextActionMethodInfo != null;

        public bool InvokeContextAction<TInvokeContext>(TInvokeContext invokeContext, out object pipelineContext)
        {
            pipelineContext = null;
            if (HasContextAction())
            {
                pipelineContext = _contextActionMethodInfo.Invoke(_contextActionObject, new object[] { invokeContext });
                return true;
            }
            return false;
        }
    }
}
