using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs;

namespace DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService
{
    public class OpenMeteoService : IWeatherForecastService
    {
        public async Task<OpenMeteoServiceResponse> GetWeatherForecastAsync(HttpClient httpClient, string apiUri)
        {
            var response = await httpClient.GetAsync(apiUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("The request to the service failed with status code: " + response.StatusCode);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true 
            };

            string responseData = await response.Content.ReadAsStringAsync();

            var openMeteoServiceResponse = JsonSerializer.Deserialize<OpenMeteoServiceResponse>(responseData, options);

            return openMeteoServiceResponse!;
        }
    }
}