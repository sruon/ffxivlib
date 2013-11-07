using System;
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
            MovementHelper mh = instance.getMovementHelper();
            mh.playWaypoint("my_waypoint");
            Console.WriteLine("Testing if blocking.");
        }
    }
}