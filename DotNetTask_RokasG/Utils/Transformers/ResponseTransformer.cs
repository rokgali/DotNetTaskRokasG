using DotNetTask_RokasG.Functionality.WeatherForecastAnalyzer;
using DotNetTask_RokasG.Services.WeatherForecast;
using DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs;
using DotNetTask_RokasG.Utils.Transformers.DTOs;
using DotNetTask_RokasG.Utils.Transformers.Strategies;

namespace DotNetTask_RokasG.Utils.Transformers
{
    public class ResponseTransformer<TInput, TOutput> : IResponseTransformer<TInput, TOutput>
    {
        private readonly ITransformationStrategy<TInput, TOutput> _transformationStrategy;
        public ResponseTransformer(ITransformationStrategy<TInput, TOutput> transformationStrategy)
        {
            _transformationStrategy = transformationStrategy;
        }

        public TOutput SelectDataForPeriod(TInput response, DateTime dateFrom, DateTime dateTo)
        {
            return _transformationStrategy.Transform(response, dateFrom, dateTo);
        }
    }
}