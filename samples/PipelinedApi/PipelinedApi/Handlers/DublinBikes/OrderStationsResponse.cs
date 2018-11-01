using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipelinedApi.Models;
using Tumble.Core;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class OrderStationsResponse : IPipelineHandler
    {
        private string[] orderIdentifiers = new string[] { "Number", "Name", "Available_bikes", "Available_bike_stands", "Last_update" };

        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out IEnumerable<DublinBikeStation> response))
            {
                
                if (context.Get("orderBy", out string value))
                {
                    var index = orderIdentifiers
                                    .Select((x, i) => new { index = i, identifer = x })
                                    .Where(x => string.Compare(x.identifer, value, true) == 0)
                                    .Select(x => x.index)
                                    .FirstOrDefault();

                    switch (index)
                    {
                        case 1:
                            response = response.OrderBy(x => x.Name);                            
                            break;
                        case 2:
                            response = response.OrderByDescending(x => x.Available_bikes);
                            break;
                        case 3:
                            response = response.OrderByDescending(x => x.Available_bike_stands);
                            break;
                        case 4:
                            response = response.OrderByDescending(x => x.Last_update);
                            break;
                        default:
                            response = response.OrderBy(x => x.Number);
                            break;
                    }

                    context.AddOrReplace("response", response.Select(x => x));                                    
                }
            }

            await next.Invoke();
        }
    }
}
