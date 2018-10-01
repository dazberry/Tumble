using PipelinedApi.Handlers;

namespace PipelinedApi.Handlers.Luas
{
    public class SetStop : SetQueryParameter
    {
        public SetStop() : base("stop", "stop", false) { }
    }
}
