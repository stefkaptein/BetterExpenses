using System.Diagnostics;
using BetterExpenses.CalculatorWorker;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NLog;
using NLog.Extensions.Logging;

var logger = LogManager.GetCurrentClassLogger();
try
{
    logger.Debug("init main");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddNLog();

    builder.Services.ConfigureServices(builder.Configuration);

    var host = builder.Build();
    host.Run();
}
catch (Exception e)
{
    logger.Fatal(e, "Application failed to startup; {Message}", e.Message);
    Debugger.Break();
}
finally
{
    LogManager.Flush();
}
