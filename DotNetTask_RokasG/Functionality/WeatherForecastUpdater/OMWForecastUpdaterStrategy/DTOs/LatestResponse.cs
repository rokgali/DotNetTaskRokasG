using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Functionality.WeatherForecastUpdater.OMWForecastUpdaterStrategy.DTOs
{
    public class LatestResponse<T>
    {
        public required bool Loading { get; set; }
        public T? Response { get; set; }
    }
}