using PipelinedApi.Handlers;

namespace PipelinedApi.Handlers.Luas
{
    public class SetAction : SetQueryParameter
    {
        public SetAction() : base("action", "action", false) { }
    }
}
