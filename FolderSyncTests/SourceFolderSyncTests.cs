/*
    * SourceFolderSyncTests.cs
    * Author: Jiri Stipek
    * Veeam test task
 */
using Veeam_test_task;

namespace FolderSyncTests
{
    public class SourceFolderSyncTests : IDisposable
    {
        private readonly string tempRoot;

        public SourceFolderSyncTests()
        {
            // Create a unique temporary root folder for testing
            tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);
        }

        // Dispose is automatically called by xUnit after each test
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
                // Ignore cleanup failures to not break test results
            }
        }

        [Fact]
        public void ValidatePath_CreatesSourceDirectory_WhenNotExists()
        {
            string sourcePath = Path.Combine(tempRoot, "SourceFolder");
            Assert.False(Directory.Exists(sourcePath));

            Validator.ValidatePath(sourcePath, Validator.PathType.Source);

            Assert.True(Directory.Exists(sourcePath));
        }

        [Fact]
        public void ValidatePath_ThrowsArgumentException_OnInvalidSourcePath()
        {
            string invalidPath = "C:\\Invalid<>Source";

            var ex = Assert.Throws<IOException>(() =>
                Validator.ValidatePath(invalidPath, Validator.PathType.Source));

            Assert.Contains("Failed to validate source path", ex.Message);
        }

        [Fact]
        public void ValidatePath_DoesNotThrow_ForExistingSourcePath()
        {
            string sourcePath = Path.Combine(tempRoot, "ExistingSource");
            Directory.CreateDirectory(sourcePath);

            var ex = Record.Exception(() =>
                Validator.ValidatePath(sourcePath, Validator.PathType.Source));

            Assert.Null(ex);
        }

        [Fact]
        public void ValidatePath_ThrowsIOException_WhenPathIsFile_ForSource()
        {
            string filePath = Path.Combine(tempRoot, "SourceAsFile.txt");
            File.WriteAllText(filePath, "dummy");

            var ex = Assert.Throws<IOException>(() =>
                Validator.ValidatePath(filePath, Validator.PathType.Source));

            Assert.Contains("Failed to validate source path", ex.Message);
        }

        [Fact]
        public void PathsAreSameOrNested_Throws_WhenPathsAreIdentical()
        {
            string samePath = Path.Combine(tempRoot, "SameFolder");
            Directory.CreateDirectory(samePath);

            var ex = Assert.Throws<ArgumentException>(() =>
                Validator.PathsAreSameOrNested(samePath, samePath));

            Assert.Contains("cannot be the same", ex.Message);
        }

        [Fact]
        public void PathsAreSameOrNested_Throws_WhenSourceInsideBackup()
        {
            string backupPath = Path.Combine(tempRoot, "BackupFolder");
            string sourcePath = Path.Combine(backupPath, "NestedSource");
            Directory.CreateDirectory(sourcePath);

            var ex = Assert.Throws<ArgumentException>(() =>
                Validator.PathsAreSameOrNested(sourcePath, backupPath));

            Assert.Contains("nested within each other", ex.Message);
        }
    }
}
