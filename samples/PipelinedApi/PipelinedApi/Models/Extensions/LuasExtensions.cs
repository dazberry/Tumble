using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelinedApi.Models.Extensions
{
    public static class LuasExtensions
    {
        public static string[] GetLineShortNames(this LuasLines stopList) =>
            stopList.Line.Select(x =>
                x.Name.Split(' ')
                .Skip(1)
                .FirstOrDefault())
            .ToArray();

        public static int GetIndexOfShortName(this LuasLines stopList, string shortName) =>
            Array.IndexOf(
                GetLineShortNames(stopList)
                    .Select(x => x.ToLower())
                    .ToArray(),
                shortName.ToLower());
                
    }
}
