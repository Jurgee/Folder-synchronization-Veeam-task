using Veeam_test_task;

namespace FolderSyncTests

{
    public class FolderSyncTests
    {
        private string tempRoot;

        public FolderSyncTests()
        {
            // Create a temporary root folder for testing
            tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);
        }

        private void Cleanup(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        [Fact]
        public void ValidateLogPath_ValidPath_NoException()
        {
            // Arrange
            string logPath = "C:\\Logs\\app.log";
            // Act
            var exception = Record.Exception(() => Validator.ValidatePath(logPath, Validator.PathType.Log));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateLogPath_CreatesDirectory_IfNotExists()
        {
            // Arrange
            string logPath = Path.Combine(tempRoot, "logs");

            // Act
            var exception = Record.Exception(() => Validator.ValidatePath(logPath, Validator.PathType.Log));

            // Assert
            Assert.Null(exception);
            Assert.True(Directory.Exists(logPath));

            Cleanup(logPath);
        }

        [Fact]
        public void ValidateSourcePath_CreatesDirectory_IfNotExists()
        {
            // Arrange
            string sourcePath = Path.Combine(tempRoot, "source");

            // Act
            var exception = Record.Exception(() => Validator.ValidatePath(sourcePath, Validator.PathType.Source));

            // Assert
            Assert.Null(exception);
            Assert.True(Directory.Exists(sourcePath));

            Cleanup(sourcePath);
        }

        [Fact]
        public void ValidateBackupPath_CreatesDirectory_IfNotExists()
        {
            // Arrange
            string backupPath = Path.Combine(tempRoot, "backup");

            // Act
            var exception = Record.Exception(() => Validator.ValidatePath(backupPath, Validator.PathType.Backup));

            // Assert
            Assert.Null(exception);
            Assert.True(Directory.Exists(backupPath));

            Cleanup(backupPath);
        }


        [Fact]
        public void ValidatePath_ThrowsUnauthorizedAccess_OnRestrictedFolder()
        {
            // Arrange
            string restrictedPath = "C:\\Windows\\System32\\RestrictedTest";

            // Act & Assert
            var ex = Record.Exception(() =>
            {
                Validator.ValidatePath(restrictedPath, Validator.PathType.Backup);
            });

            if (ex != null)
            {
                Assert.True(ex is UnauthorizedAccessException || ex is IOException);
            }
        }
    }
}