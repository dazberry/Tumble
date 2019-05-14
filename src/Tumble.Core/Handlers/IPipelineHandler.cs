using System.Threading.Tasks;

namespace Tumble.Core
{
    public delegate Task PipelineDelegate();    
    
    public interface IPipelineHandler<T> 
    {
        Task InvokeAsync(PipelineDelegate next, T context);
    }

    public interface IPipelineHandler<T,T1>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1);
    }

    public interface IPipelineHandler<T, T1, T2>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1, T2 context2);
    }

    public interface IPipelineHandler<T, T1, T2, T3>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1, T2 context2, T3 context3);
    }

    public interface IPipelineHandler<T, T1, T2, T3, T4>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1, T2 context2, T3 context3, T4 context4);
    }

    public interface IPipelineHandler<T, T1, T2, T3, T4, T5>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5);
    }

    public interface IPipelineHandler<T, T1, T2, T3, T4, T5, T6>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6);
    }

    public interface IPipelineHandler<T, T1, T2, T3, T4, T5, T6, T7>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7);
    }

    public interface IPipelineHandler<T, T1, T2, T3, T4, T5, T6, T7, T8>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7, T8 context8);
    }

    public interface IPipelineHandler<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        Task InvokeAsync(PipelineDelegate next, T context, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7, T8 context8, T9 context9);
    }





}
