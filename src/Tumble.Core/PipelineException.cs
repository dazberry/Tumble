using System;
using System.Linq;

namespace Tumble.Core
{
    public class PipelineException : Exception        
    {
        public PipelineException(string message = "", Exception innerException = null)
            : base(message, innerException) { }           
    }


    public class PipelineHandlerException<THandler> : PipelineException
    {
        private THandler PipelineHandler { get; }

        public PipelineHandlerException(THandler pipelineHandler, string message, Exception innerException = null)
            : base(message, innerException)
        {
            PipelineHandler = pipelineHandler;
        }
    }

    public class PipelineContextException<TContext> : PipelineException
    {
        private TContext PipelineContext { get; }

        public PipelineContextException(TContext pipelineContext, string message, Exception innerException)
            : base(message, innerException)
        {
            PipelineContext = pipelineContext;
        }
    }
}
