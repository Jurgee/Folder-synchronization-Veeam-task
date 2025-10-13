/*
 * Logger.cs
 * Used external library: Serilog (https://github.com/serilog/serilog
 * Author: Jiri Stipek
 * Veeam test task
 * Configure Serilog for logging to console and file
 */
using Serilog;

namespace Veeam_test_task
{
    public class Logger
    {
        /// <summary>
        /// Configure Serilog to log to console and to a file with daily rolling and size limit
        /// </summary>
        /// <param name="filePath"></param>
        public static void Configure(string filePath)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(filePath, "log.txt"),
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }
    }
}
