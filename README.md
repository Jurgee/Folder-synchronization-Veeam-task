# Folder synchronization tool
## Veeam Test Task
### Author: Jiří Štípek

### Description
This project implements a simple folder synchronization tool that copies files from a source folder to a backup folder at specified intervals. It also logs actions and errors using Serilog.

### Features
- Periodic synchronization of files from a source folder to a backup folder.
- Logging of actions and errors using Serilog.
- Automatically creates directories if they don’t exist.
- Configurable synchronization interval.
- Graceful shutdown on user input.
- Validates source, backup, and log paths.

### Prerequisites
- NuGet packages:
  - Serilog
  - Serilog.Sinks.File
  - Serilog.Sinks.Console
- Command-line interface
- File system access

### Arguments in command line
- `-s or --source <path>`: Path to the source folder
- `-b or --backup <path>`: Path to the backup folder
- `-l or --log <path>`: Path to the log folder (file will be created inside this folder)
- `-i or --interval <seconds>`: Synchronization interval in seconds

### Setup Instructions
1. **Clone the repository**
```bash
git clone https://github.com/Jurgee/Veeam-Test-task.git
```
2. **Navigate to the project directory**
```bash
cd FolderSync
```

4. **Build the application**
```bash
dotnet build
```
6. **Run the application**
```bash
dotnet run -- -s <source_path> -b <backup_path> -l <log_path> -i <interval_in_seconds>
```

### Example Usage
```bash
dotnet run -- -s "C:\SourceFolder" -b "C:\BackupFolder" -l "C:\Logs" -i 60
```

### Logging
Logs will be written to the specified log file and also output to the console. The log will include information about synchronization actions and any errors encountered.

### Graceful Shutdown
Press **Ctrl+C** to stop the synchronization process gracefully.

### 3rd Party Libraries
- [Serilog](https://github.com/serilog/serilog): A diagnostic logging library for .NET applications.
- [CommandLineParser](https://github.com/commandlineparser/commandline?tab=readme-ov-file) : A library for parsing command line arguments.
- [System.Timers](https://learn.microsoft.com/cs-cz/dotnet/api/system.timers.timer?view=net-9.0) : A timer that raises an event at user-defined intervals.