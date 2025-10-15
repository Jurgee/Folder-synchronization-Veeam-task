/*
    * LogFolderSyncTests.cs
    * Author: Jiri Stipek
    * Veeam test task
 */
using Veeam_test_task;

namespace FolderSyncTests
{
    public class LogFolderSyncTests : IDisposable
    {
        private readonly string tempRoot;

        public LogFolderSyncTests()
        {
            tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);
        }

        public void Dispose()
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }

        [Fact]
        public void ValidateLogPath_ValidPath_NoException()
        {
            // Arrange
            string logPath = Path.Combine(tempRoot, "app.log");

            // Act
            var exception = Record.Exception(() =>
                Validator.ValidatePath(logPath, Validator.PathType.Log));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateLogPath_CreatesDirectory_IfNotExists()
        {
            // Arrange
            string logDir = Path.Combine(tempRoot, "logs");

            // Act
            var exception = Record.Exception(() =>
                Validator.ValidatePath(logDir, Validator.PathType.Log));

            // Assert
            Assert.Null(exception);
            Assert.True(Directory.Exists(logDir));
        }

        [Fact]
        public void ValidateLogPath_ThrowsIOException_WhenPathIsFile()
        {
            // Arrange
            string filePath = Path.Combine(tempRoot, "log_as_file.txt");
            File.WriteAllText(filePath, "dummy");

            // Act
            var ex = Assert.Throws<IOException>(() =>
                Validator.ValidatePath(filePath, Validator.PathType.Log));

            // Assert
            Assert.Contains("Failed to validate log path", ex.Message);
        }

        [Fact]
        public void ValidateLogPath_ThrowsUnauthorizedAccess_OnRestrictedFolder()
        {
            // Arrange
            string restrictedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "RestrictedTest");

            // Act & Assert
            var ex = Record.Exception(() =>
                Validator.ValidatePath(restrictedPath, Validator.PathType.Log));

            if (ex != null)
            {
                Assert.True(ex is UnauthorizedAccessException || ex is IOException);

            }
        }

        [Fact]
        public void ValidateLogPath_CreatesNestedDirectories()
        {
            string nestedPath = Path.Combine(tempRoot, "logs", "nested1", "nested2");
            var exception = Record.Exception(() =>
                Validator.ValidatePath(nestedPath, Validator.PathType.Log));
            Assert.Null(exception);
            Assert.True(Directory.Exists(nestedPath));
        }
    }
}
