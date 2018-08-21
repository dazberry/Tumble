namespace Tumble.Core
{
    public interface IPipelineContextItem
    {
        string Name { get; }    
        bool IsNamed { get; }

        bool Is<T>();
        T As<T>();
    }
}
