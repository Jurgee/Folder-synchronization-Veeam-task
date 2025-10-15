using Veeam_test_task;

namespace FolderSyncTests
{
    public class ArgumentsSyncTests
    {
        private readonly ArgumentParser parser = new();

        [Fact]
        public void ParseArguments_ValidArgs_ReturnsExpectedValues()
        {
            // Arrange
            string[] args = { "-s", "C:\\Source", "-b", "C:\\Backup", "-l", "C:\\Logs\\app.log", "-i", "30" };

            // Act
            var result = parser.ParseArguments(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C:\\Source", result.SourceFolder);
            Assert.Equal("C:\\Backup", result.BackupFolder);
            Assert.Equal("C:\\Logs\\app.log", result.LogFile);
            Assert.Equal(30, result.Interval);
        }

        [Fact]
        public void ParseArguments_MissingRequiredArg_ReturnsNull()
        {
            // Arrange (missing -b / backup)
            string[] args = { "-s", "C:\\Source", "-l", "C:\\Logs\\app.log", "-i", "10" };

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            var result = parser.ParseArguments(args);

            // Assert
            Assert.Null(result);
            string output = sw.ToString();
            Assert.Contains("Failed to parse command-line arguments", output);
        }

        [Fact]
        public void ParseArguments_InvalidInterval_ReturnsNull()
        {
            // Arrange
            string[] args = { "-s", "C:\\Source", "-b", "C:\\Backup", "-l", "C:\\Logs\\app.log", "-i", "abc" };

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            var result = parser.ParseArguments(args);

            // Assert
            Assert.Null(result);
            Assert.Contains("Failed to parse command-line arguments", sw.ToString());
        }

        [Fact]
        public void ParseArguments_MissingAllArgs_ReturnsNull()
        {
            string[] args = { };

            using var sw = new StringWriter();
            Console.SetOut(sw);

            var result = parser.ParseArguments(args);

            Assert.Null(result);
            Assert.Contains("Failed to parse command-line arguments", sw.ToString());
        }

        [Fact]
        public void ParseArguments_ExtraUnknownArg_ReturnsNull()
        {
            string[] args = { "-s", "C:\\Source", "-b", "C:\\Backup", "-l", "C:\\Logs\\app.log", "-i", "10", "-x", "something" };

            using var sw = new StringWriter();
            Console.SetOut(sw);

            var result = parser.ParseArguments(args);

            Assert.Null(result);
            Assert.Contains("Failed to parse command-line arguments", sw.ToString());
        }

        [Fact]
        public void ParseArguments_ValidArgs_WithDifferentOrder_Works()
        {
            string[] args = { "-i", "5", "-b", "D:\\Backup", "-s", "D:\\Source", "-l", "D:\\Logs\\log.txt" };

            var result = parser.ParseArguments(args);

            Assert.NotNull(result);
            Assert.Equal("D:\\Source", result.SourceFolder);
            Assert.Equal("D:\\Backup", result.BackupFolder);
            Assert.Equal("D:\\Logs\\log.txt", result.LogFile);
            Assert.Equal(5, result.Interval);
        }
        [Fact]
        public void ValidateInterval_ThrowsArgumentException_WhenNonPositive()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                Validator.ValidateInterval(-1));

            Assert.Contains("Interval must be a positive integer", ex.Message);
        }
    }
}
