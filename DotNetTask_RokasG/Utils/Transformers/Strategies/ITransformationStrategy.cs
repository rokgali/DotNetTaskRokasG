using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Utils.Transformers.Strategies
{
    public interface ITransformationStrategy<TInput, TOutput>
    {
        TOutput Transform(TInput response, DateTime dateFrom, DateTime dateTo);
    }
}
