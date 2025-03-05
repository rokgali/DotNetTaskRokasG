using DotNetTask_RokasG.Utils.Transformers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Functionality.WeatherForecastAnalyzer
{
    public interface IWeatherForecastAnalysisStrategy<T>
    {
        public bool WillGetWet(int precipitation);
        public bool ShouldTakeUmbrella(T TransformedResponseList);
    }
}