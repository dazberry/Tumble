namespace PipelinedApi.Handlers
{
    public class SetMaxResults : SetQueryParameter
    {
        public SetMaxResults() : base("maxResults", "maxResults", false) { }
    }
}
