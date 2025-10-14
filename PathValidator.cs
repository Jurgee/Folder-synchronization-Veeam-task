/*
 * PathValidator.cs
 * Author: Jiri Stipek
 * Veeam test task
 * Validate paths and ensure the directory exists or create it
 */
namespace Veeam_test_task
{
    public class PathValidator
    {
        /// <summary>
        /// Validate the log file path and ensure the directory exists or create it
        /// </summary>
        /// <param name="logFilePath"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateLogPath(string logFilePath)
        {
            string? logDir = Path.GetDirectoryName(logFilePath);

            if (logDir == null || logDir.IndexOfAny(Path.GetInvalidPathChars()) >= 0) // Check for invalid path characters
                throw new ArgumentException($"Invalid path: {logFilePath}");


            if (!Directory.Exists(logDir)) // Create directory if it doesn't exist
            {
                Directory.CreateDirectory(logDir);
                Console.WriteLine($"Created log directory: {logDir}");
            }





        }

        public static bool ValidateAndPrepare(ArgumentParser.Options options)
        {
            bool isValid = true;

            // Source folder must exist
            if (!Directory.Exists(options.SourceFolder))
            {
                Console.WriteLine($"Error: Source folder does not exist: {options.SourceFolder}");
                isValid = false;
            }

            // Create backup folder if missing
            try
            {
                if (!Directory.Exists(options.BackupFolder))
                {
                    Directory.CreateDirectory(options.BackupFolder);
                    Console.WriteLine($"Created backup folder: {options.BackupFolder}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Cannot create backup folder: {ex.Message}");
                isValid = false;
            }

            // Prevent same or nested paths
            if (isValid && AreSameOrNested(options.SourceFolder, options.BackupFolder))
            {
                Console.WriteLine("Error: Source and backup folders cannot be the same or nested.");
                isValid = false;
            }

            // Interval
            if (options.Interval <= 0)
            {
                Console.WriteLine($"Error: Invalid interval {options.Interval}. Must be positive.");
                isValid = false;
            }

            // Prepare log file
            try
            {
                string? logDir = Path.GetDirectoryName(options.LogFile);
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                    Console.WriteLine($"Created log directory: {logDir}");
                }

                if (!File.Exists(options.LogFile))
                {
                    File.Create(options.LogFile).Dispose();
                    Console.WriteLine($"Created new log file: {options.LogFile}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Cannot create or access log file: {ex.Message}");
                isValid = false;
            }

            return isValid;
        }

        private static bool AreSameOrNested(string path1, string path2)
        {
            string full1 = Path.GetFullPath(path1);
            string full2 = Path.GetFullPath(path2);

            if (full1.Equals(full2, StringComparison.OrdinalIgnoreCase))
                return true;

            return full1.StartsWith(full2 + Path.DirectorySeparatorChar) ||
                   full2.StartsWith(full1 + Path.DirectorySeparatorChar);
        }
    }
}
