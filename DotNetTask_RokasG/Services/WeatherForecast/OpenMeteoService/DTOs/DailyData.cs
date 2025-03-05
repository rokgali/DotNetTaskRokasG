using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs
{
    public record DailyData
    {
        public required List<DateTime> Time { get; init; }
        public required List<int> Precipitation_Probability_Max { get; init; }
    }
}