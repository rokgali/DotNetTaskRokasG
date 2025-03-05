using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DotNetTask_RokasG.Constants.ApiUris;
using DotNetTask_RokasG.Functionality.WeatherForecastAnalyzer;
using DotNetTask_RokasG.Functionality.WeatherForecastUpdater;
using DotNetTask_RokasG.Utils.Transformers.DTOs;

namespace DotNetTask_RokasG.WeatherClient
{
    class OpenMeteoWeatherClient
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWeatherForecastUpdaterStrategy<OpenMeteoTransformedReponseList> _weatherForecastUpdater;
        private readonly IWeatherForecastAnalysisStrategy<OpenMeteoTransformedReponseList> _weatherForecastAnalyzer;
        
        private bool isUserWaitingForData;
        private string header = $"{"Time",-20}{"Temperature (°C)",-20}{"Precip. Prob. (%)",-20}{"Precipitation (mm)",-20}{"Rain (mm)",-20}{"Snowfall (mm)",-20}{"Getting Wet Likelihood",-20}";

        public OpenMeteoWeatherClient(
            IWeatherForecastUpdaterStrategy<OpenMeteoTransformedReponseList> weatherForecastUpdater,
            IServiceProvider serviceProvider,
            IWeatherForecastAnalysisStrategy<OpenMeteoTransformedReponseList> weatherForecastAnalyzer)
        {
            _serviceProvider = serviceProvider;
            _weatherForecastUpdater = weatherForecastUpdater;
            _weatherForecastAnalyzer = weatherForecastAnalyzer;

            _weatherForecastUpdater.DataRefreshed += OnDataRefreshed!;
        }

        private void OnDataRefreshed(object sender, EventArgs e)
        {
            if (!isUserWaitingForData)
                return;

            Task.Run(() =>
            {
                var latestData = _weatherForecastUpdater.GetLatestWeatherForecast().Response;

                lock (Console.Out)
                {
                    Console.WriteLine("\nWeather data refreshed!");

                    Console.WriteLine(header);
                    foreach (var hourlyResponse in latestData!.TransformedHourlyResponses)
                    {
                        Console.WriteLine(hourlyResponse);
                    }

                    Console.WriteLine(_weatherForecastAnalyzer.ShouldTakeUmbrella(latestData)
                        ? "You should take an umbrella"
                        : "You shouldn't take an umbrella");
                }
            });
        }


        public async Task StartAsync()
        {
            while (true)
            {
                isUserWaitingForData = false;

                Console.WriteLine("\nPress 'y' to get weather data, 'q' to quit.");
                var input = Console.ReadKey(intercept: true).KeyChar;

                if (input == 'q')
                {
                    Console.WriteLine("\nExiting...");
                    break;
                }

                if (input == 'y')
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userLocationOrDefault = UserInputHandler.GetUserLocationOrDefault();
                        string apiUri = string.Format(
                            CultureInfo.InvariantCulture,
                            ApiUris.DEFAULT_OPEN_METEO_URI_TEMPLATE,
                            userLocationOrDefault.Latitude,
                            userLocationOrDefault.Longitude);

                        await _weatherForecastUpdater.SetApiUriAsync(apiUri);

                        isUserWaitingForData = true;

                        while (_weatherForecastUpdater.GetLatestWeatherForecast().Loading)
                        {
                            await Task.Delay(1000);
                            Console.WriteLine("Data is loading...");
                        }

                        var result = _weatherForecastUpdater.GetLatestWeatherForecast().Response!;

                        Console.WriteLine(header);
                        result.TransformedHourlyResponses.ForEach(hourlyResponse =>
                        {
                            Console.WriteLine(hourlyResponse);
                        });

                        Console.WriteLine(_weatherForecastAnalyzer.ShouldTakeUmbrella(result)
                            ? "You should take an umbrella"
                            : "You shouldn't take an umbrella");

                        while (true)
                        {
                            Console.WriteLine("\nPress 'b' to go back, 'r' to refresh data or 'q' to quit.");
                            var subInput = Console.ReadKey(intercept: true).KeyChar;

                            if (subInput == 'r')
                            {
                                await _weatherForecastUpdater.RefreshWeatherForecastDataAsync();
                                Console.WriteLine("\nData has been refreshed!");

                                Console.WriteLine(header);
                                result.TransformedHourlyResponses.ForEach(hourlyResponse =>
                                {
                                    Console.WriteLine(hourlyResponse);
                                });

                                Console.WriteLine(_weatherForecastAnalyzer.ShouldTakeUmbrella(result)
                                    ? "You should take an umbrella"
                                    : "You shouldn't take an umbrella");
                            }
                            else if (subInput == 'b')
                            {
                                break;
                            }
                            else if (subInput == 'q')
                            {
                                Console.WriteLine("\nExiting...");
                                return;
                            }
                            else
                            {
                                Console.WriteLine("\nInvalid input. Please press 'b' to go back or 'q' to quit.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid input. Please press 'y', 'r', or 'q'.");
                }
            }
        }
    }
}
