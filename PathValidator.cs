/*
 * PathValidator.cs
 * Author: Jiri Stipek
 * Veeam test task
 * Validate paths and ensure the directory exists or create it
 */

using Serilog;

namespace Veeam_test_task
{
    public class PathValidator
    {
        public enum PathType
        {
            Log,
            Source,
            Backup
        }
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
    }
}
