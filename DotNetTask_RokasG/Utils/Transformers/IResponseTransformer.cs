using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetTask_RokasG.Utils.Transformers.DTOs;

namespace DotNetTask_RokasG.Utils.Transformers
{
    public interface IResponseTransformer<TInput, TOutput>
    {
        public TOutput SelectDataForPeriod(TInput response, DateTime dateFrom, DateTime dateTo);
    }
}