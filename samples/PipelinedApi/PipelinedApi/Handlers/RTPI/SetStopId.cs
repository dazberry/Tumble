using PipelinedApi.Handlers;

namespace PipelinedApi.Handlers.Rtpi
{
    public class SetStopId : SetQueryParameter
    {
        public SetStopId() : base("stopId", "stopId", false) { }
    }
}
