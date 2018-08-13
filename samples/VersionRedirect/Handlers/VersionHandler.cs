using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tumble.Core;

namespace VersionRedirect.Handlers
{    
    public class VersionHandler : IPipelineHandler
    {
        private (bool, string) GetVersionHeader(IHeaderDictionary headerDictionary)
        {
            var versionHeader = headerDictionary.FirstOrDefault(x => string.Compare(x.Key, "version", true) == 0);
            if (versionHeader.Key == null)
                return (false, string.Empty);
            var versionNo = versionHeader.Value.First();
            return (!string.IsNullOrEmpty(versionNo), versionNo);        
        }

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (!context.GetFirst(out HttpContext httpContext))
                throw new PipelineDependencyException<HttpContext>(this);

            var (result, versionNo) = GetVersionHeader(httpContext.Request.Headers);
            if (result)
                context.Add(versionNo);
            
            await next.Invoke();            
        }
    }
}
