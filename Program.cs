using Serilog;

namespace Veeam_test_task
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var parser = new ArgumentParser();
            var result = parser.ParseArguments(args);
            var syncTimer = new System.Timers.Timer();
            if (result == null)
            {
                Console.WriteLine("Argument parsing failed. Please check the provided arguments.");
                return;
            }

            try
            {
                // Configure logger
                Logger.Configure(result.LogFile);

                // Validate log
                Validator.ValidatePath(result.LogFile, Validator.PathType.Log);
                Log.Information("Log path validated at {LogPath}", result.LogFile);

                // Validate interval
                Validator.ValidateInterval(result.Interval);
                Log.Information("Interval validated at {Interval} seconds", result.Interval);

                // Validate source and backup paths
                Validator.PathsAreSameOrNested(result.SourceFolder, result.BackupFolder);
                Validator.ValidatePath(result.SourceFolder, Validator.PathType.Source);
                Log.Information("Source path validated at {SourcePath}", result.SourceFolder);
                Validator.ValidatePath(result.BackupFolder, Validator.PathType.Backup);
                Log.Information("Backup path validated at {BackupPath}", result.BackupFolder);

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
            }
            finally
            {
                Log.Information("Synchronization process completed, disposing resources.");
                Log.Information("-----------Synchronization stopped-----------");
                Log.CloseAndFlush();
                syncTimer.Dispose();
            }
        }
    }
}