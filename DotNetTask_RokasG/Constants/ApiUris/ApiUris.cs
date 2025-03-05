using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DotNetTask_RokasG.Constants.DefaultLocations;

namespace DotNetTask_RokasG.Constants.ApiUris
{
    public class ApiUris
    {
        public const string DEFAULT_OPEN_METEO_URI_TEMPLATE = "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&hourly=temperature_2m,precipitation_probability,precipitation,rain,snowfall&daily=precipitation_probability_max&timezone=auto&forecast_days=2";
        public static readonly string DEFAULT_OPEN_METEO_URI_KAUNAS = $"https://api.open-meteo.com/v1/forecast?latitude={Locations.KAUNAS.Latitude.ToString(CultureInfo.InvariantCulture)}&longitude={Locations.KAUNAS.Longitude.ToString(CultureInfo.InvariantCulture)}&hourly=temperature_2m,precipitation_probability,precipitation,rain,snowfall&daily=precipitation_probability_max&timezone=auto&forecast_days=2";
        public static readonly string DEFAULT_OPEN_METEO_URI_VILNIUS = $"https://api.open-meteo.com/v1/forecast?latitude={Locations.VILNIUS.Latitude.ToString(CultureInfo.InvariantCulture)}&longitude={Locations.VILNIUS.Longitude.ToString(CultureInfo.InvariantCulture)}&hourly=temperature_2m,precipitation_probability,precipitation,rain,snowfall&daily=precipitation_probability_max&timezone=auto&forecast_days=2";
    }
}