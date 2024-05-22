using Serilog;
using Serilog.Events;
namespace ZGRemote.Common.Logging
{
    public static class Logger
    {
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log.txt", LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();
        }
    }

}