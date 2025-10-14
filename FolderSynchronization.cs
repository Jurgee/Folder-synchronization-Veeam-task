/*
    * FolderSynchronization.cs
    * Author: Jiri Stipek
    * Veeam test task
    * Synchronize source and backup folders
*/
using Serilog;

namespace Veeam_test_task
{
    internal class FolderSynchronization
    {
        /// <summary>
        /// Synchronize source and backup folders
        /// </summary>
        public static void SyncFolders(string source, string backup)
        {
            DirectoryInfo diSource = new DirectoryInfo(source);
            DirectoryInfo diBackup = new DirectoryInfo(backup);

            SyncAll(diSource, diBackup);
            DeleteSyncAll(diSource, diBackup);
        }

        /// <summary>
        /// Copy all files and directories from source to backup, preserving structure and only updating newer files
        /// </summary>
        /// <param name="diSource"></param>
        /// <param name="diBackup"></param>
        public static void SyncAll(DirectoryInfo diSource, DirectoryInfo diBackup)
        {
            foreach (FileInfo files in diSource.GetFiles()) // Go through each file in the source directory
            {
                string backupFilePath = Path.Combine(diBackup.FullName, files.Name);
                DateTime sourceLastModify = files.LastWriteTime;

                if (File.Exists(backupFilePath)) // Check if the file already exists in the backup directory
                {
                    FileInfo backupFile = new FileInfo(backupFilePath);
                    DateTime backupLastModify = backupFile.LastWriteTime;

                    try
                    {
                        if (sourceLastModify > backupLastModify)
                        {
                            Log.Information("Updating {File} (newer source version found)", files.FullName);
                            files.CopyTo(backupFilePath, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, @"Error occurred while processing file {0}\{1}", files.DirectoryName, files.Name);
                    }
                }

                else
                {
                    Log.Information("Copying new file {File} -> {Dest}", files.FullName, backupFilePath);
                    files.CopyTo(backupFilePath, true);
                }
            }

            foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories()) // Go through each subdirectory in the source directory
            {
                string backupSubDirPath = Path.Combine(diBackup.FullName, diSourceSubDir.Name);
                DirectoryInfo nextTargetSubDir = new DirectoryInfo(backupSubDirPath);

                if (!nextTargetSubDir.Exists)
                {
                    nextTargetSubDir.Create();
                    Log.Information("Created folder {Folder}", nextTargetSubDir.FullName);
                }

                // Recurse into the subdirectory
                SyncAll(diSourceSubDir, nextTargetSubDir);
            }

        }

        /// <summary>
        /// Delete files and directories from backup that no longer exist in source
        /// </summary>
        /// <param name="diSource"></param>
        /// <param name="diBackup"></param>
        public static void DeleteSyncAll(DirectoryInfo diSource, DirectoryInfo diBackup)
        {
            foreach (FileInfo backupFile in diBackup.GetFiles()) // Go through each file in the backup directory
            {
                string sourceFilePath = Path.Combine(diSource.FullName, backupFile.Name);
                if (!File.Exists(sourceFilePath))
                {
                    try
                    {
                        Log.Information("Deleting file {File} (no longer in source)", backupFile.FullName);
                        backupFile.Delete();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to delete file {File}", backupFile.FullName);
                    }
                }
            }

            foreach (DirectoryInfo diBackupSubDir in diBackup.GetDirectories()) // Go through each subdirectory in the backup directory
            {
                string sourceSubDirPath = Path.Combine(diSource.FullName, diBackupSubDir.Name);
                if (!Directory.Exists(sourceSubDirPath)) // If the subdirectory does not exist in the source, delete it from the backup
                {
                    try
                    {
                        Log.Information("Deleting folder {Folder} (no longer exists in source)", diBackupSubDir.FullName);
                        diBackupSubDir.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to delete folder {Folder}", diBackupSubDir.FullName);
                    }
                }
                else
                {
                    DirectoryInfo diSourceSubDir = new DirectoryInfo(sourceSubDirPath);
                    DeleteSyncAll(diSourceSubDir, diBackupSubDir);
                }
            }
        }

    }
}
