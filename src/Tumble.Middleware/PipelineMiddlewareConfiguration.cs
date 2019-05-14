namespace Tumble.Middleware
{
    public class PipelineMiddlewareConfiguration
    {
        public string StartsWithSegment { get; set; } = "/api";

        public PipelineMiddlewareAfterInvoke AfterPipelineInvoke { get; set; } = PipelineMiddlewareAfterInvoke.Exit;        
    }
}
