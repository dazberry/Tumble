using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipelinedApi.Models;
using Tumble.Core;

namespace PipelinedApi.Handlers.DublinBikes
{
    public interface IDublinBikeStationsContext
    {
        IEnumerable<DublinBikeStation> DublinBikeStations { get; set; }
    }

    public interface IDublinBikeStationsOrderByContext
    {
        string OrderBy { get; set; }
    }

    public class OrderStationsResponse : IPipelineHandler<IDublinBikeStationsContext, IDublinBikeStationsOrderByContext>
    {
        private string[] orderIdentifiers = new string[] { "Number", "Name", "Available_bikes", "Available_bike_stands", "Last_update" };

        public async Task InvokeAsync(PipelineDelegate next, 
            IDublinBikeStationsContext dbsContext,
            IDublinBikeStationsOrderByContext orderContext)
        {
            var stations = dbsContext.DublinBikeStations;
            if (stations?.Any() ?? false)
            {
                var index = orderIdentifiers
                        .Select((x, i) => new { index = i, identifer = x })
                        .Where(x => string.Compare(x.identifer, orderContext.OrderBy, true) == 0)
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
                dbsContext.DublinBikeStations = stations;
            }                     

            await next.Invoke();
        }
    }
}
