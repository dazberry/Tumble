using System;
using System.Threading.Tasks;
using Tumble.Core;

namespace Tumble.Handlers.Miscellaneous
{
    public class ActionHandler<TContext> : IPipelineHandler<TContext>        
    {
        private readonly Func<TContext, PipelineDelegate, Task> _action;

        public ActionHandler(Func<TContext, PipelineDelegate, Task> action) =>
            _action = action;

        public Task InvokeAsync(PipelineDelegate next, TContext context)
        {
            _action?.Invoke(context, next);
            return next.Invoke();
        }
    }
        
}
