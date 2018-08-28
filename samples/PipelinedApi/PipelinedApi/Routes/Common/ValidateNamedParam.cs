using System;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Routes.Common
{
    public class ValidateNamedParam<T> : IPipelineHandler
    {
        public string ParamName { get; set; }
        public Func<T, bool> ValidateAction { get; set; }        

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (!string.IsNullOrEmpty(ParamName))
            {
                var result = context.Get(ParamName, out T value);
                if (result && ValidateAction != null)
                {
                    result = ValidateAction.Invoke(value);
                }
                if (result)
                {
                    await next.Invoke();
                }
                else
                {
                    context.AddNotification(this, $"Parameter \"{ParamName}\" failed validation");
                }
            }
            else
                context.AddNotification(this, $"Missing parameter \"{ParamName}\"");
        }
    }
}
