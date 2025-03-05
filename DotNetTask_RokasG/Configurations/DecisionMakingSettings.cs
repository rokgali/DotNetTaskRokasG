using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Configurations
{
    public class DecisionMakingSettings
    {
        public int PrecipitationThresholdPercentage { get; set; }
        public int PrecipitationLeadTime { get; set; }
        public int RefreshFrequencyMinutes { get; set; }
    }
}