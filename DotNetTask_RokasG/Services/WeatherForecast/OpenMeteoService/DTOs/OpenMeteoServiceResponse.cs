using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs
{
    public record OpenMeteoServiceResponse
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public required string Timezone { get; init; }
        public required HourlyData Hourly { get; init; }
        public required DailyData Daily { get; init; }
    }
}