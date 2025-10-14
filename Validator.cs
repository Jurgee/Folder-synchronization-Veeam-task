/*
 * PathValidator.cs
 * Author: Jiri Stipek
 * Veeam test task
 * Validate paths and ensure the directory exists or create it
 */

using Serilog;

namespace Veeam_test_task
{
    public class Validator
    {
        /// <summary>
        /// Path types for validation context
        /// </summary>
        public enum PathType
        {
            Log,
            Source,
            Backup
        }

        /// <summary>
        /// Validate a given path for invalid characters and ensure the directory exists or create it
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="Exception"></exception>
        public static void ValidatePath(string path, PathType type)
        {
            string fullPath = Path.GetFullPath(path);

            if (fullPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException($"Invalid {type.ToString().ToLower()} path: {path}");

            try
            {
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    Log.Information($"Created {type.ToString().ToLower()} directory: {fullPath}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException(
                    $"Access denied to {type.ToString().ToLower()} path: {path}");
            }
            catch (IOException ex)
            {
                throw new IOException(
                    $"Failed to validate {type.ToString().ToLower()} path '{path}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Unexpected error while validating {type.ToString().ToLower()} path '{path}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if two paths are the same or nested within each other
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void PathsAreSameOrNested(string path1, string path2)
        {
            string full1 = Path.GetFullPath(path1);
            string full2 = Path.GetFullPath(path2);

            if (full1.Equals(full2, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Source and backup paths cannot be the same.");
            if (full1.StartsWith(full2 + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                full2.StartsWith(full1 + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Source and backup paths cannot be nested within each other.");
        }

        public static void ValidateInterval(int interval)
        {
            if (interval <= 0)
                throw new ArgumentException("Interval must be a positive integer.");
        }
    }
}
