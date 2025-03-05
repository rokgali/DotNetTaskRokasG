using DotNetTask_RokasG.Constants.DefaultLocations;
using DotNetTask_RokasG.Functionality.WeatherForecastUpdater.OMWForecastUpdaterStrategy.DTOs;
using DotNetTask_RokasG.UserInput;
using DotNetTask_RokasG.Utils.Transformers.DTOs;

namespace DotNetTask_RokasG.Functionality.WeatherForecastUpdater
{
    public interface IWeatherForecastUpdaterStrategy<T>
    {
        public event EventHandler DataRefreshed;
        public Task SetApiUriAsync(string apiUri);
        public LatestResponse<T> GetLatestWeatherForecast();
        public Task RefreshWeatherForecastDataAsync();
    }
}