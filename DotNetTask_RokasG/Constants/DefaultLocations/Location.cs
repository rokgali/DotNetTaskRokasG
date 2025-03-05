using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Constants.DefaultLocations
{
    public record Location
    {
        public required string Name { get; init; }
        public required float Latitude { get; init; }
        public required float Longitude { get; init; }
    }
}