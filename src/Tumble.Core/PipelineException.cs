using System;
using System.Linq;

namespace Tumble.Core
{
    public class PipelineException : Exception
    {
        IPipelineHandler PipelineHandler { get; }

        public PipelineException(IPipelineHandler pipelineHandler, string message, Exception innerException = null) 
            : base(message, innerException)
        {
            PipelineHandler = pipelineHandler;
        }
    }

    public class PipelineDependencyException : PipelineException
    {
        protected static string FormatErrorMessage(IPipelineHandler handler, params string[] dependencies)
        {
            var handlerType = handler == null ? "Handler" : handler.GetType().Name;
            var dependencyText = !dependencies.Any()
                ? "is missing a dependency"
                : "is missing dependenc" +
                    (dependencies.Count() == 1
                    ? $"y - {dependencies[0]}"
                    : $"ies - {string.Join(",", dependencies)}");
            return $"{handlerType} {dependencyText}";
        }

        public PipelineDependencyException(IPipelineHandler handler, params string[] dependencies)
            : base(handler, FormatErrorMessage(handler, dependencies))
        {

        }
    }

    public class PipelineDependencyException<T> : PipelineDependencyException
    {
        public PipelineDependencyException(IPipelineHandler handler)
            : base(handler, FormatErrorMessage(handler,
                typeof(T).Name))
        {
        }
    }
}
