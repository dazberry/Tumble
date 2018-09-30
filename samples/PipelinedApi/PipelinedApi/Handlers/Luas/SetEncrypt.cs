using PipelinedApi.Handlers;

namespace PipelinedApi.Handlers.Luas
{
    public class SetEncrypt : SetQueryParameter
    {
        public SetEncrypt() : base("encrypt", "encrypt", false) { }
    }
}
