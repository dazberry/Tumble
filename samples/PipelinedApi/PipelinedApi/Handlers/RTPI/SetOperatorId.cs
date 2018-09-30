using PipelinedApi.Handlers;

namespace PipelinedApi.Handlers.Rtpi
{
    public class SetOperatorId : SetQueryParameter
    {
        public SetOperatorId() : base("operatorId", "operator", false) { }       
    }
}
