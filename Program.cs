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
                PathValidator.ValidateLogPath(result.LogFile);
                Logger.Configure(result.LogFile);
                Log.Information("Log initialized.");

            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: Access denied to path '{result.LogFile}'. {ex.Message}");
                return;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: IO exception while validating log path '{result.LogFile}'. {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while initializing log path: {ex.Message}");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}