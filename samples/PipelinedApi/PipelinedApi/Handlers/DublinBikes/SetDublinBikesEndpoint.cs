using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class SetDublinBikesEndpoint : SetEndpointHandler
    {       
        public override Uri BaseUrl { get => base.BaseUrl; set => base.BaseUrl = value; }
    }
}
