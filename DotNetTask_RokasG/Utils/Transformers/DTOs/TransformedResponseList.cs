using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Utils.Transformers.DTOs
{
    public abstract class TransformedResponseList<T> where T : TransformedHourlyResponse
    {
        public required List<T> TransformedHourlyResponses { get; set; }
    }
}
