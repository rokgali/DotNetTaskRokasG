using DotNetTask_RokasG.Functionality.WeatherForecastAnalyzer;
using DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs;
using DotNetTask_RokasG.Utils.Transformers.DTOs;
using DotNetTask_RokasG.Utils.Transformers.Strategies;

public class OpenMeteoResponseTransformationStrategy : ITransformationStrategy<OpenMeteoServiceResponse, OpenMeteoTransformedReponseList>
{
    private readonly IWeatherForecastAnalysisStrategy<OpenMeteoTransformedReponseList> _weatherForecastAnalyzer;

    public OpenMeteoResponseTransformationStrategy(IWeatherForecastAnalysisStrategy<OpenMeteoTransformedReponseList> weatherForecastAnalyzer)
    {
        _weatherForecastAnalyzer = weatherForecastAnalyzer;
    }

    public OpenMeteoTransformedReponseList Transform(OpenMeteoServiceResponse response, DateTime dateFrom, DateTime dateTo)
    {
        var transformedHourlyResponses = response.Hourly.Time
        .Select((time, i) => new OpenMeteoTransformedHourlyResponse
        {
            Time = time,
            Temperature_2m = response.Hourly.Temperature_2m[i],
            Precipitation_Probability = response.Hourly.Precipitation_Probability[i],
            Precipitation = response.Hourly.Precipitation[i],
            Rain = response.Hourly.Rain[i],
            Snowfall = response.Hourly.Snowfall[i],
            GettingWetLikelyhood = _weatherForecastAnalyzer.WillGetWet(response.Hourly.Precipitation_Probability[i])
        })
        .Where(hourlyResponse => dateFrom <= hourlyResponse.Time && hourlyResponse.Time <= dateTo)
        .ToList();
       
        return new OpenMeteoTransformedReponseList { TransformedHourlyResponses = transformedHourlyResponses };
    }
}
