/*
    * Program.cs
    * Author: Jiri Stipek
    * Veeam test task
    * Main program class for Veeam test task
*/
using Serilog;

namespace Veeam_test_task
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var parser = new ArgumentParser();
            var result = parser.ParseArguments(args);
            var syncTimer = new System.Timers.Timer();
            if (result is null)
            {
                return 1;
            }

            try
            {
                // Configure logger
                Logger.Configure(result.LogFile);

                // Validate log
                Validator.ValidatePath(result.LogFile, Validator.PathType.Log);

                // Validate interval
                Validator.ValidateInterval(result.Interval);

                // Validate source and backup paths
                Validator.PathsAreSameOrNested(result.SourceFolder, result.BackupFolder);
                Validator.ValidatePath(result.SourceFolder, Validator.PathType.Source);
                Validator.ValidatePath(result.BackupFolder, Validator.PathType.Backup);

                // Start synchronization
                syncTimer = Timer.SetTimer(result.Interval, result.SourceFolder, result.BackupFolder);

                Log.Information("-----------Start synchronization-----------");
                var quitEvent = new ManualResetEvent(false);

                // Handle Ctrl+C to exit app
                Console.CancelKeyPress += (sender, eArgs) =>
                {
                    eArgs.Cancel = true;
                    quitEvent.Set();
                };
                quitEvent.WaitOne();
                syncTimer.Stop();

            }

            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
                return 1;
            }
            finally
            {
                Log.Information("Synchronization process completed, disposing resources.");
                Log.Information("-----------Synchronization stopped-----------");
                Log.CloseAndFlush();
                syncTimer.Dispose();
            }

            return 0;
        }
    }
}