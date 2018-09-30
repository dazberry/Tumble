using PipelinedApi.Handlers;

namespace PipelinedApi.Handlers.Rtpi
{
    public class SetMaxResults : SetQueryParameter
    {
        public SetMaxResults() : base("maxResults", "maxResults", false) { }
    }
}
