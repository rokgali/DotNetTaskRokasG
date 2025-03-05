using DotNetTask_RokasG.Configurations;
using DotNetTask_RokasG.Functionality.WeatherForecastAnalyzer;
using DotNetTask_RokasG.Functionality.WeatherForecastAnalyzer.Strategies;
using DotNetTask_RokasG.Functionality.WeatherForecastUpdater;
using DotNetTask_RokasG.Functionality.WeatherForecastUpdater.OpenMeteoWeatherForecastUpdater;
using DotNetTask_RokasG.Services.WeatherForecast;
using DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService;
using DotNetTask_RokasG.Services.WeatherForecast.OpenMeteoService.DTOs;
using DotNetTask_RokasG.Utils.Transformers;
using DotNetTask_RokasG.Utils.Transformers.DTOs;
using DotNetTask_RokasG.Utils.Transformers.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Serilog;
using DotNetTask_RokasG.Utils.Loggers.ApiLogger;
using DotNetTask_RokasG.Constants.FilePaths;
using DotNetTask_RokasG.WeatherClient;

var logger = new LoggerConfiguration()
    .WriteTo.File(LogFilePaths.API_RESPONSE_FILE_PATH, rollingInterval: RollingInterval.Day)
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) => {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {   
        services.AddScoped<IWeatherForecastUpdaterStrategy<OpenMeteoTransformedReponseList>, OMWForecastUpdaterStrategy>();
        services.AddScoped<IWeatherForecastAnalysisStrategy<OpenMeteoTransformedReponseList>, OMWAnalysisStrategy>();

        services.AddTransient<IWeatherForecastService, OpenMeteoService>();

        services.AddTransient<ITransformationStrategy<OpenMeteoServiceResponse, OpenMeteoTransformedReponseList>, OpenMeteoResponseTransformationStrategy>();
        services.AddTransient<IResponseTransformer<OpenMeteoServiceResponse, OpenMeteoTransformedReponseList>, ResponseTransformer<OpenMeteoServiceResponse, OpenMeteoTransformedReponseList>>();
        services.AddTransient<IApiLogger, ApiLogger>();

        services.AddTransient<HttpClient>();
        
        services.Configure<DecisionMakingSettings>(hostContext.Configuration.GetSection("DecisionMakingSettings"));

        services.AddSingleton<IOptionsMonitor<DecisionMakingSettings>>(sp =>
        {
            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<DecisionMakingSettings>>();
            
            return optionsMonitor;
        });
        services.AddSingleton<OpenMeteoWeatherClient>();
    }).ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSerilog(logger);
    })
    .Build();

    var openMeteoWeatherClient = host.Services.GetService<OpenMeteoWeatherClient>();

    await openMeteoWeatherClient!.StartAsync();









