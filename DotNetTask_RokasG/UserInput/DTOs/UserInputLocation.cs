using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.UserInput.DTOs
{
    public record UserInputLocation
    {
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public float Longitude { get; set; }
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public float Latitude { get; set; }
    }
}
