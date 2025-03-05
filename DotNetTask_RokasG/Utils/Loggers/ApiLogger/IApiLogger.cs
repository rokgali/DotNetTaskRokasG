using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Utils.Loggers.ApiLogger
{
    public interface IApiLogger
    {
        void LogApiResponse(string responseMessage);
    }
}
