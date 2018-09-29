using Tumble.Core;
using Tumble.Core.Notifications;
using Tumble.Handlers.Miscellaneous;

namespace PipelinedApi.Routes
{
    public class RoutePipelines
    {
        PipelineRequestCollection _pipelines = new PipelineRequestCollection();
                 
        public bool Add(string name, PipelineRequest pipelineRequest) =>        
            _pipelines.Add(name, pipelineRequest);

        private PipelineRequest _nullPipelineRequest = 
            new PipelineRequest()
                .AddDelegateHandler(ctx => ctx.AddNotification(null, "Unknown pipeline route"));
            
        public PipelineRequest this[string name] =>
            _pipelines.Get(name) ?? _nullPipelineRequest;
    }
}
