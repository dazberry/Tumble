using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelinedApi.Handlers.RTPI
{
    public class SetRTPIEndpoint : SetEndpointHandler
    {
        public override Uri BaseUrl { get => base.BaseUrl; set => base.BaseUrl = value; }
    }
}
