using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTask_RokasG.Utils.Loggers.ApiLogger
{
    public class ApiLogger : IApiLogger
    {
        private readonly ILogger<ApiLogger> _logger;
        public ApiLogger(ILogger<ApiLogger> logger)
        {
            _logger = logger;
        }

        public void LogApiResponse(string responseMessage)
        {
            _logger.LogInformation($"{DateTime.Now} API Response: {responseMessage}");
        }
    }
}
