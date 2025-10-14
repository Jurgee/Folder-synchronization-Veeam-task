/*
* Timer.cs
 * Used external library: System.Timers (https://learn.microsoft.com/cs-cz/dotnet/api/system.timers.timer?view=net-9.0)
* Author: Jiri Stipek
* Veeam test task
* Set up a timer to trigger synchronization at specified intervals
*/
using System.Timers;

namespace Veeam_test_task
{
    public class Timer
    {
        /// <summary>
        /// Set up a timer to trigger synchronization at specified intervals
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static System.Timers.Timer SetTimer(int interval)
        {
            var syncTimer = new System.Timers.Timer(interval * 1000);
            syncTimer.Elapsed += OnTimedEvent;
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
            return syncTimer;

        }

        // TODO: Implement actual synchronization logic here
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                e.SignalTime);
        }
    }
}
