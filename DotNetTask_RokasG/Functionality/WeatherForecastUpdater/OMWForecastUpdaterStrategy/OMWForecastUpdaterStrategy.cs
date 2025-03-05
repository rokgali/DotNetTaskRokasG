using System.Globalization;
using System.Threading.Tasks;
using DotNetTask_RokasG.Configurations;
using DotNetTask_RokasG.Constants.ApiUris;
using DotNetTask_RokasG.Functionality.WeatherForecastUpdater.OMWForecastUpdaterStrategy.DTOs;
using DotNetTask_RokasG.Services.WeatherForecast;
using DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs;
using DotNetTask_RokasG.UserInput;
using DotNetTask_RokasG.Utils.Loggers.ApiLogger;
using DotNetTask_RokasG.Utils.Transformers;
using DotNetTask_RokasG.Utils.Transformers.DTOs;
using Microsoft.Extensions.Options;

namespace DotNetTask_RokasG.Functionality.WeatherForecastUpdater.OpenMeteoWeatherForecastUpdater
{
    public class OMWForecastUpdaterStrategy : IWeatherForecastUpdaterStrategy<OpenMeteoTransformedReponseList>
    {
        private readonly IWeatherForecastService _openMeteoService;
        private readonly HttpClient _httpClient;
        private readonly IResponseTransformer<OpenMeteoServiceResponse, OpenMeteoTransformedReponseList> _transformer;
        private readonly IApiLogger _apiLogger;
        private readonly IOptionsSnapshot<DecisionMakingSettings> _decisionMakingSettings;

        private OpenMeteoServiceResponse? openMeteoServiceResponse;
        private LatestResponse<OpenMeteoTransformedReponseList>? OldResponse;
        private LatestResponse<OpenMeteoTransformedReponseList> LatestResponse;

        private Timer? Timer { get; set; }
        private string? ApiUri { get; set; }
        public (DateTime, DateTime)? DataPeriod { get; set; }

        public event EventHandler? DataRefreshed;

        public OMWForecastUpdaterStrategy(IWeatherForecastService openMeteoService,
            HttpClient httpClient, IResponseTransformer<OpenMeteoServiceResponse,
            OpenMeteoTransformedReponseList> transformer,
            IApiLogger apiLogger,
            IOptionsSnapshot<DecisionMakingSettings> decisionMakingSettings)
        {
            _openMeteoService = openMeteoService;
            _httpClient = httpClient;
            _transformer = transformer;
            _apiLogger = apiLogger;
            _decisionMakingSettings = decisionMakingSettings;

            LatestResponse = new() { Loading = true };

            DataPeriod = DataPeriod == default ? (DateTime.Now.AddHours(-1), DateTime.Now.AddDays(1)) : DataPeriod;

            Timer = new Timer(_ =>
            {
                Task.Run(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(ApiUri))
                    {
                        await SetLatestWeatherForecastAsync();

                        OnDataRefreshed();
                    }
                });
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(decisionMakingSettings.Value.RefreshFrequencyMinutes));
        }

        protected virtual void OnDataRefreshed()
        {
            DataRefreshed?.Invoke(this, EventArgs.Empty);
        }

        private async Task SetLatestWeatherForecastAsync()
        {
            try
            {
                await UpdateWeatherAsync();

                var transformedResponse = _transformer.SelectDataForPeriod(openMeteoServiceResponse!, DataPeriod!.Value.Item1, DataPeriod.Value.Item2);

                OldResponse = LatestResponse;
                LatestResponse = new() { Loading = false, Response = transformedResponse };

                _apiLogger.LogApiResponse($"Data fetched successfully");
            }
            catch (Exception ex)
            {
                if (OldResponse?.Response != null)
                {
                    LatestResponse = new()
                    {
                        Loading = false,    
                        Response = OldResponse.Response
                    };
                }
                else
                {
                    LatestResponse = new()
                    {
                        Loading = false,
                    };
                }

                Console.WriteLine("An error occurred, please check the logs for more information");
                 _apiLogger.LogApiResponse($"Error while updating weather: {ex.Message}");
            }
        }

        private async Task UpdateWeatherAsync()
        {
            LatestResponse = new() { Loading = true };

            openMeteoServiceResponse = await _openMeteoService.GetWeatherForecastAsync(_httpClient, ApiUri!);
        }

        public LatestResponse<OpenMeteoTransformedReponseList> GetLatestWeatherForecast()
        {
            return LatestResponse;
        }

        public async Task RefreshWeatherForecastDataAsync()
        {
            await SetLatestWeatherForecastAsync();
        }

        public async Task SetApiUriAsync(string apiUri)
        {
            ApiUri = apiUri;

            await RefreshWeatherForecastDataAsync();
        }
    }
}