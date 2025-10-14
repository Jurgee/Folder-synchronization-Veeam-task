using Serilog;

namespace Veeam_test_task
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new ArgumentParser();
            var result = parser.ParseArguments(args);

            if (result == null)
            {
                Console.WriteLine("Argument parsing failed. Please check the provided arguments.");
                return;
            }

            try
            {
                Logger.Configure(result.LogFile);
                PathValidator.ValidatePath(result.LogFile, PathValidator.PathType.Log);
                PathValidator.PathsAreSameOrNested(result.SourceFolder, result.BackupFolder);

                Log.Information("Log path validated at {LogPath}", result.LogFile);
                PathValidator.ValidatePath(result.SourceFolder, PathValidator.PathType.Source);
                Log.Information("Source path validated at {SourcePath}", result.SourceFolder);
                PathValidator.ValidatePath(result.BackupFolder, PathValidator.PathType.Backup);
                Log.Information("Backup path validated at {BackupPath}", result.BackupFolder);
            }

            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}