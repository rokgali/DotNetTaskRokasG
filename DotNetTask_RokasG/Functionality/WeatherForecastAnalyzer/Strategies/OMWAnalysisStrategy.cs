using DotNetTask_RokasG.Configurations;
using DotNetTask_RokasG.Utils.Transformers.DTOs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Functionality.WeatherForecastAnalyzer.Strategies
{
    public class OMWAnalysisStrategy : IWeatherForecastAnalysisStrategy<OpenMeteoTransformedReponseList>
    {
        private readonly IOptionsSnapshot<DecisionMakingSettings> _decisionMakingSettings;
        public OMWAnalysisStrategy(IOptionsSnapshot<DecisionMakingSettings> decisionMakingSettings)
        {
            _decisionMakingSettings = decisionMakingSettings;
        }
        public bool ShouldTakeUmbrella(OpenMeteoTransformedReponseList transformedResponseList)
        {
            return transformedResponseList.TransformedHourlyResponses.Exists(pred => pred.GettingWetLikelyhood == true && DateTime.Now.AddDays(-1) <= pred.Time && pred.Time <= DateTime.Now.AddDays(_decisionMakingSettings.Value.PrecipitationLeadTime));
        }

        public bool WillGetWet(int precipitation)
        {
            return precipitation >= _decisionMakingSettings.Value.PrecipitationThresholdPercentage;
        }
    }
}
