using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Xunit;
using DotNetTask_RokasG.Functionality.WeatherForecastUpdater.OpenMeteoWeatherForecastUpdater;
using DotNetTask_RokasG.Services.WeatherForecast;
using DotNetTask_RokasG.Utils.Transformers;
using DotNetTask_RokasG.Utils.Transformers.DTOs;
using DotNetTask_RokasG.Utils.Loggers.ApiLogger;
using DotNetTask_RokasG.UserInput;
using DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs;
using System.Reflection;
using DotNetTask_RokasG.Configurations;
using Microsoft.Extensions.Options;
using DotNetTask_RokasG.Functionality.WeatherForecastUpdater.OMWForecastUpdaterStrategy.DTOs;

namespace DotNetTask_RokasG.IntegrationTests.WeatherAnalysStrategies
{
    public class OMWForecastUpdaterStrategyIntegrationTests
    {
        private readonly Mock<IWeatherForecastService> _mockWeatherService;
        private readonly Mock<IResponseTransformer<OpenMeteoServiceResponse, OpenMeteoTransformedReponseList>> _mockTransformer;
        private readonly Mock<IApiLogger> _mockLogger;
        private readonly HttpClient _httpClient;
        private OMWForecastUpdaterStrategy _updater;
        private readonly Mock<IOptionsSnapshot<DecisionMakingSettings>> _mockOptionsSnapshot;

        public OMWForecastUpdaterStrategyIntegrationTests()
        {
            _mockOptionsSnapshot = new Mock<IOptionsSnapshot<DecisionMakingSettings>>();
            _mockOptionsSnapshot
                .Setup(options => options.Value)
                .Returns(new DecisionMakingSettings { PrecipitationThresholdPercentage = 10, PrecipitationLeadTime = 2, RefreshFrequencyMinutes = 1 });
            _mockWeatherService = new Mock<IWeatherForecastService>();
            _mockTransformer = new Mock<IResponseTransformer<OpenMeteoServiceResponse, OpenMeteoTransformedReponseList>>();
            _mockLogger = new Mock<IApiLogger>();
            _httpClient = new HttpClient();
            _updater = new OMWForecastUpdaterStrategy(_mockWeatherService.Object, _httpClient, _mockTransformer.Object, _mockLogger.Object, _mockOptionsSnapshot.Object);
        }

        [Fact]
        public async Task UpdateWeatherAsync_FailedApiCall_KeepsOldResponse()
        {
            var oldResponse = new OpenMeteoTransformedReponseList
            {
                TransformedHourlyResponses = new List<OpenMeteoTransformedHourlyResponse>
                {
                    new OpenMeteoTransformedHourlyResponse
                    {
                        Time = DateTime.Now.AddHours(-1),
                        Temperature_2m = 15.0f,
                        Precipitation_Probability = 20,
                        Precipitation = 0.0f,
                        Rain = 0.0f,
                        Snowfall = 0.0f,
                        GettingWetLikelyhood = false
                    }
                }
            };

            _updater.GetType()
                .GetField("OldResponse", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_updater, new LatestResponse<OpenMeteoTransformedReponseList> { Loading = false, Response = oldResponse });

            _mockWeatherService
                .Setup(service => service.GetWeatherForecastAsync(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("API Error"));

            _updater.GetType().GetField("ApiUri", BindingFlags.NonPublic | BindingFlags.Instance)?
            .SetValue(_updater, "https://mock-api.com");


            await _updater.RefreshWeatherForecastDataAsync();

            Assert.False(_updater.GetLatestWeatherForecast().Loading);
            Assert.Equal(oldResponse, _updater.GetLatestWeatherForecast().Response);
        }

        [Fact]
        public async Task Timer_ShouldRunAndCallServiceWhenApiUriIsSet()
        {
            await _updater.SetApiUriAsync("https://mock-api.com");

            await Task.Delay(2000);

            _mockWeatherService.Verify(service => service.GetWeatherForecastAsync(It.IsAny<HttpClient>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task SetLatestWeatherForecastAsync_ShouldUpdateWeather_WhenApiUriIsValid()
        {
            var expectedApiUri = "https://example.com/api/forecast";

            var hourlyData = new HourlyData
            {
                Time = new List<DateTime> { new DateTime(2025, 3, 5, 0, 0, 0), new DateTime(2025, 3, 5, 1, 0, 0) },
                Temperature_2m = new List<float> { 10.5f, 12.3f },
                Precipitation_Probability = new List<int> { 80, 70 },
                Precipitation = new List<float> { 1.5f, 0.8f },
                Rain = new List<float> { 1.0f, 0.5f },
                Snowfall = new List<float> { 0.0f, 0.0f }
            };

            var dailyData = new DailyData
            {
                Time = new List<DateTime> { new DateTime(2025, 3, 5), new DateTime(2025, 3, 6) },
                Precipitation_Probability_Max = new List<int> { 90, 50 }
            };

            var openMeteoServiceResponse = new OpenMeteoServiceResponse
            {
                Latitude = 52.3794,
                Longitude = 13.0624,
                Timezone = "Europe/Berlin",
                Hourly = hourlyData,
                Daily = dailyData
            };

            var transformedResponse = new OpenMeteoTransformedReponseList
            {
                TransformedHourlyResponses = new List<OpenMeteoTransformedHourlyResponse>
        {
            new OpenMeteoTransformedHourlyResponse
            {
                Time = DateTime.Parse("2025-03-05T00:00"),
                Temperature_2m = 10.5f,
                Precipitation_Probability = 80,
                Precipitation = 1.5f,
                Rain = 1.0f,
                Snowfall = 0.0f,
                GettingWetLikelyhood = true
            },
            new OpenMeteoTransformedHourlyResponse
            {
                Time = DateTime.Parse("2025-03-05T01:00"),
                Temperature_2m = 12.3f,
                Precipitation_Probability = 70,
                Precipitation = 0.8f,
                Rain = 0.5f,
                Snowfall = 0.0f,
                GettingWetLikelyhood = false
            }
        }
            };

            _mockWeatherService
                .Setup(service => service.GetWeatherForecastAsync(It.IsAny<HttpClient>(), It.IsAny<string>()))
                .ReturnsAsync(openMeteoServiceResponse);

            _mockTransformer
                .Setup(transformer => transformer.SelectDataForPeriod(It.IsAny<OpenMeteoServiceResponse>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(transformedResponse);

            var decisionMakingSettings = new DecisionMakingSettings { RefreshFrequencyMinutes = 5 };
            _mockOptionsSnapshot.Setup(o => o.Value).Returns(decisionMakingSettings);

            var strategy = new OMWForecastUpdaterStrategy(
                _mockWeatherService.Object,
                _httpClient,
                _mockTransformer.Object,
                _mockLogger.Object,
                _mockOptionsSnapshot.Object);

            await strategy.SetApiUriAsync(expectedApiUri);

            await Task.Delay(1000);

            var latestWeatherForecast = strategy.GetLatestWeatherForecast();
            Assert.False(latestWeatherForecast.Loading, "Weather data is still loading.");
            Assert.NotNull(latestWeatherForecast.Response);

            var actualResponse = latestWeatherForecast.Response.TransformedHourlyResponses;

            Assert.Equal(2, actualResponse.Count);

            Assert.Equal(transformedResponse.TransformedHourlyResponses[0].Time, actualResponse[0].Time);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[0].Temperature_2m, actualResponse[0].Temperature_2m);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[0].Precipitation_Probability, actualResponse[0].Precipitation_Probability);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[0].Precipitation, actualResponse[0].Precipitation);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[0].Rain, actualResponse[0].Rain);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[0].Snowfall, actualResponse[0].Snowfall);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[0].GettingWetLikelyhood, actualResponse[0].GettingWetLikelyhood);

            Assert.Equal(transformedResponse.TransformedHourlyResponses[1].Time, actualResponse[1].Time);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[1].Temperature_2m, actualResponse[1].Temperature_2m);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[1].Precipitation_Probability, actualResponse[1].Precipitation_Probability);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[1].Precipitation, actualResponse[1].Precipitation);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[1].Rain, actualResponse[1].Rain);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[1].Snowfall, actualResponse[1].Snowfall);
            Assert.Equal(transformedResponse.TransformedHourlyResponses[1].GettingWetLikelyhood, actualResponse[1].GettingWetLikelyhood);
        }
    }
}
