using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipelinedApi.Models;
using Tumble.Client.Parameters;
using Tumble.Core;
using Tumble.Core.Contexts;

namespace PipelinedApi.Handlers.DublinBikes
{
    public class OrderStationsResponse : IPipelineHandler<IContextResolver<IEnumerable<DublinBikeStation>>, IQueryParameters>
    {              
        private string[] orderIdentifiers = new string[] { "Number", "Name", "Available_bikes", "Available_bike_stands", "Last_update" };

        public async Task InvokeAsync(PipelineDelegate next,
            IContextResolver<IEnumerable<DublinBikeStation>> dublinBikeStations,
            IQueryParameters queryParameters)
        {
            var stations = dublinBikeStations.Get();
            if (stations?.Any() ?? false)
            {
                var orderBy = queryParameters.Get()
                    .Where(x => string.Compare(x.Name, "orderBy", true) == 0)
                    .FirstOrDefault()?.Value;

                var index = orderIdentifiers
                        .Select((x, i) => new { index = i, identifer = x })
                        .Where(x => string.Compare(x.identifer, orderBy, true) == 0)
                        .Select(x => x.index)
                        .FirstOrDefault();

                switch (index)
                {
                    case 1:
                        stations = stations.OrderBy(x => x.Name);
                        break;
                    case 2:
                        stations = stations.OrderByDescending(x => x.Available_bikes);
                        break;
                    case 3:
                        stations = stations.OrderByDescending(x => x.Available_bike_stands);
                        break;
                    case 4:
                        stations = stations.OrderByDescending(x => x.Last_update);
                        break;
                    default:
                        stations = stations.OrderBy(x => x.Number);
                        break;
                }
                dublinBikeStations.Set(stations);
            }                     

            await next.Invoke();
        }
    }
}
