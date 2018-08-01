using System.Threading.Tasks;

namespace Tumble.Core
{
    public delegate Task PipelineDelegate();

    public interface IPipelineHandler
    {
        Task InvokeAsync(PipelineContext context, PipelineDelegate next);
    }

}
