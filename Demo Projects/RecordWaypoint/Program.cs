using System.Threading;
using ffxivlib;

namespace RecordWaypoint
{
    internal class Program
    {
        /// <summary>
        /// Instantiates a movementhelper, start recording 
        /// positions for 30 seconds to a file named my_waypoint.
        /// Copy that file over to the program called PlayWaypoint
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            MovementHelper mh = instance.GetMovementHelper();
            mh.StartRecordingCoordinates("my_waypoint");
            Thread.Sleep(30000);
            mh.StopRecordingWaypoint();
        }
    }
}