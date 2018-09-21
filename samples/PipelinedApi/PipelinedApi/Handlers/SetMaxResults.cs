using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Tumble.Core;
using Tumble.Core.Notifications;

namespace PipelinedApi.Handlers
{
    public class SetMaxResults : SetQueryParameter
    {
        public SetMaxResults() : base("maxResults", "maxResults", false) { }
    }
}
