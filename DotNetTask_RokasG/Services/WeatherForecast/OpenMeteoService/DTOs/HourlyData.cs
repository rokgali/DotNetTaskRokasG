using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs
{
    public record HourlyData
    {
        public required List<DateTime> Time { get; init; }
        public required List<float> Temperature_2m { get; init; }
        public required List<int> Precipitation_Probability { get; init; }
        public required List<float> Precipitation { get; init; }
        public required List<float> Rain { get; init; }
        public required List<float> Snowfall { get; init; }
    }
}