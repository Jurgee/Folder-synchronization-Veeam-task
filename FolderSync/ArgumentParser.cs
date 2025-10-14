/*
    * ArgumentParser.cs
    * Used external library: CommandLineParser (https://github.com/commandlineparser/commandline?tab=readme-ov-file)
    * Author: Jiri Stipek
    * Veeam test task
    * Parse command-line arguments
*/
using CommandLine;

namespace Veeam_test_task
{
    public class ArgumentParser
    {
        /// <summary>
        /// Options class to hold command-line arguments
        /// </summary>
        public class Options
        {
            [Option('s', "source", Required = true, HelpText = "Path to the source folder")]
            public required string SourceFolder { get; set; }

            [Option('b', "backup", Required = true, HelpText = "Path to the backup folder")]
            public required string BackupFolder { get; set; }

            [Option('i', "interval", Required = true, HelpText = "Interval for periodic synchronizations in seconds")]
            public int Interval { get; set; }

            [Option('l', "log", Required = true, HelpText = "Path to the log file")]
            public required string LogFile { get; set; }
        }
        /// <summary>
        /// Parse command-line arguments using CommandLineParser library
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Options? ParseArguments(string[] args)
        {
            Options? parsed = null;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    parsed = o;
                })
                .WithNotParsed(errors =>
                {
                    Console.WriteLine("Failed to parse command-line arguments.");
                });

            return parsed;

        }

    }
}