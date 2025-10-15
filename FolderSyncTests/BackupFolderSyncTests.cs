using Veeam_test_task;

namespace FolderSyncTests
{
    public class BackupFolderSyncTests : IDisposable
    {
        private readonly string tempRoot;

        public BackupFolderSyncTests()
        {
            tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);
        }

        public void Dispose()
        {
            Cleanup(tempRoot);
        }

        private void Cleanup(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch
            {
                // Ignore cleanup failures to prevent false negatives
            }
        }

        [Fact]
        public void ValidatePath_CreatesBackupDirectory_WhenNotExists()
        {
            string backupPath = Path.Combine(tempRoot, "BackupFolder");
            Assert.False(Directory.Exists(backupPath));

            Validator.ValidatePath(backupPath, Validator.PathType.Backup);

            Assert.True(Directory.Exists(backupPath));
        }

        [Fact]
        public void ValidatePath_ThrowsArgumentException_OnInvalidBackupPath()
        {
            string invalidPath = "C:\\Invalid?Path";

            var ex = Assert.Throws<IOException>(() =>
                Validator.ValidatePath(invalidPath, Validator.PathType.Backup));

            Assert.Contains("Failed to validate backup path", ex.Message);
        }

        [Fact]
        public void ValidatePath_DoesNotThrow_ForExistingBackupPath()
        {
            string backupPath = Path.Combine(tempRoot, "ExistingBackup");
            Directory.CreateDirectory(backupPath);

            var ex = Record.Exception(() =>
                Validator.ValidatePath(backupPath, Validator.PathType.Backup));

            Assert.Null(ex);
        }

        [Fact]
        public void ValidatePath_ReadOnlyBackupPath_NoException()
        {
            string path = Path.Combine(tempRoot, "ReadonlyBackup");
            Directory.CreateDirectory(path);
            var dirInfo = new DirectoryInfo(path);
            dirInfo.Attributes |= FileAttributes.ReadOnly;

            try
            {
                var ex = Record.Exception(() =>
                    Validator.ValidatePath(path, Validator.PathType.Backup));
                Assert.Null(ex);
            }
            finally
            {
                dirInfo.Attributes &= ~FileAttributes.ReadOnly;
            }
        }

        [Fact]
        public void ValidatePath_ThrowsIOException_WhenPathIsFile_ForBackup()
        {
            string filePath = Path.Combine(tempRoot, "BackupAsFile.txt");
            File.WriteAllText(filePath, "dummy");

            var ex = Assert.Throws<IOException>(() =>
                Validator.ValidatePath(filePath, Validator.PathType.Backup));

            Assert.Contains("Failed to validate backup path", ex.Message);
        }

        [Fact]
        public void PathsAreSameOrNested_Throws_WhenBackupInsideSource()
        {
            string sourcePath = Path.Combine(tempRoot, "SourceFolder");
            string backupPath = Path.Combine(sourcePath, "NestedBackup");
            Directory.CreateDirectory(backupPath);

            var ex = Assert.Throws<ArgumentException>(() =>
                Validator.PathsAreSameOrNested(sourcePath, backupPath));

            Assert.Contains("nested within each other", ex.Message);
        }

        [Fact]
        public void PathsAreSameOrNested_DoesNotThrow_WhenIndependent()
        {
            string sourcePath = Path.Combine(tempRoot, "SourceA");
            string backupPath = Path.Combine(tempRoot, "BackupB");
            Directory.CreateDirectory(sourcePath);
            Directory.CreateDirectory(backupPath);

            var ex = Record.Exception(() =>
                Validator.PathsAreSameOrNested(sourcePath, backupPath));

            Assert.Null(ex);
        }
    }
}
