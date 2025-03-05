using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Utils.Transformers.DTOs
{
    public abstract class TransformedHourlyResponse
    {
        public required DateTime Time { get; set; }
        public required float Temperature_2m { get; set; }
        public required int Precipitation_Probability { get; set; }
        public required float Precipitation { get; set; }
        public required float Rain { get; set; }
        public required float Snowfall { get; set; }
        public required bool GettingWetLikelyhood { get; set; }

        public override string ToString()
        {
            // Define column widths for each field
            const int timeWidth = 20;
            const int tempWidth = 20;
            const int precipProbWidth = 20;
            const int precipWidth = 20;
            const int rainWidth = 20;
            const int snowWidth = 20;
            const int wetLikelihoodWidth = 20;

            return $"{Time:yyyy-MM-dd HH:mm}".PadRight(timeWidth) +
                   $"{Temperature_2m:0.0}°C".PadRight(tempWidth) +
                   $"{Precipitation_Probability}%".PadRight(precipProbWidth) +
                   $"{Precipitation:0.0}mm".PadRight(precipWidth) +
                   $"{Rain:0.0}mm".PadRight(rainWidth) +
                   $"{Snowfall:0.0}mm".PadRight(snowWidth) +
                   $"{(GettingWetLikelyhood ? "High" : "Low")}".PadRight(wetLikelihoodWidth);
        }

    }
}
