using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelinedApi.Handlers.Luas
{
    /// <summary>    
    /// Sets Context.Uri from BaseUrl
    /// <para></para>        
    /// Requires: IContextWriter&lt;Uri&gt;
    /// </summary>  
    public class SetLuasEndpoint : SetEndpointHandler
    {
        public override Uri BaseUrl { get => base.BaseUrl; set => base.BaseUrl = value; }
    }
}
