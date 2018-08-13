namespace Tumble.Middleware
{
    public class PipelineMiddlewareConfiguration
    {
        public string StartsWithSegment { get; set; } = "/api";

        public PipelineMiddlewareEnum AfterPipelineInvoke { get; set; } = PipelineMiddlewareEnum.Exit;        
    }
}
