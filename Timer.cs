/*
    * Timer.cs
    * Used external library: System.Timers (https://learn.microsoft.com/cs-cz/dotnet/api/system.timers.timer?view=net-9.0)
    * Author: Jiri Stipek
    * Veeam test task
    * Set up a timer to trigger synchronization at specified intervals
*/
namespace Veeam_test_task
{
    public class Timer
    {
        /// <summary>
        /// Set up a timer to trigger synchronization at specified intervals
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static System.Timers.Timer SetTimer(int interval, string source, string backup)
        {
            var syncTimer = new System.Timers.Timer(interval * 1000);
            syncTimer.Elapsed += (sender, e) => FolderSynchronization.SyncFolders(source, backup);
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
            return syncTimer;
        }
    }
}
