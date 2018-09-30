using PipelinedApi.Handlers;

namespace PipelinedApi.Handlers.Rtpi
{
    public class SetRouteId : SetQueryParameter
    {
        public SetRouteId() : base("routeId", "routeId", false) { }
    }
}
