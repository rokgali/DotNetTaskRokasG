using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs;

namespace DotNetTask_RokasG.Services.WeatherForecast
{
    public interface IWeatherForecastService
    {
        public Task<OpenMeteoServiceResponse> GetWeatherForecastAsync(HttpClient httpClient, string apiUri);
    }
}