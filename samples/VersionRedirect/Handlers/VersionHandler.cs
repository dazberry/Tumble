using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tumble.Core;
using VersionRedirect.Contexts;

namespace VersionRedirect.Handlers
{
    /// <summary>    
    /// Requires: VersionContext. Sets VersionNumber from Header["version"].
    /// </summary>
    public class VersionHandler : IPipelineHandler<VersionContext>
    {
        private (bool, string) GetVersionHeader(IHeaderDictionary headerDictionary)
        {
            var versionHeader = headerDictionary.FirstOrDefault(x => string.Compare(x.Key, "version", true) == 0);
            if (versionHeader.Key == null)
                return (false, string.Empty);
            var versionNo = versionHeader.Value.First();
            return (!string.IsNullOrEmpty(versionNo), versionNo);        
        }

        public async Task InvokeAsync(PipelineDelegate next, VersionContext context)
        {            
            var (result, versionNo) = GetVersionHeader(context.HttpContext.Request.Headers);
            if (result)
                context.VersionNumber = versionNo;
                        
            await next.Invoke();            
        }
    }
}
