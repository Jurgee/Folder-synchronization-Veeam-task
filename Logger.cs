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
            // Create a unique file name with timestamp
            string logFileName = $"log_{DateTime.Now:yyyyMMdd_HHmm}.txt";
            string logFilePath = Path.Combine(filePath, logFileName);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logFilePath,
                    rollingInterval: RollingInterval.Infinite,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }
    }
}
