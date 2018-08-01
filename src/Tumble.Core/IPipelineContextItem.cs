namespace Tumble.Core
{
    public interface IPipelineContextItem
    {
        bool Is<T>();
        T As<T>();
    }
}
