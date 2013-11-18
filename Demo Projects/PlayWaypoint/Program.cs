using System.Threading;
using ffxivlib;

namespace PlayWaypoint
{
    internal class Program
    {
        /// <summary>
        /// Please run the RecordWaypoint program first 
        /// and copy the file generated over to this demo.
        /// We instantiate a MovementHelper instance and pass the filename to be played.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            MovementHelper mh = instance.GetMovementHelper();
            mh.PlayWaypoint("my_waypoint");
            // Run for 5 seconds, then pause the running for 10 seconds
            Thread.Sleep(5000);
            mh.PauseWaypoint();
            Thread.Sleep(10000);
            mh.PauseWaypoint();
        }
    }
}