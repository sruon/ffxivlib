using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ffxivlib
{
    /// <summary>
    /// Credits to FFACETOOLS.
    /// </summary>
    public class MovementHelper
    {
        private volatile bool _stop;
        private readonly Entity player;
        private readonly SendKeyInput ski = SendKeyInput.getInstance();
        private List<Coords> coords_list = new List<Coords>();
        private readonly Serializer serializer = new Serializer();
        private readonly SendKeyInput.VKKeys left_key;
        private readonly SendKeyInput.VKKeys right_key;
        private readonly SendKeyInput.VKKeys forward_key;

        internal MovementHelper(Entity _player,
                                SendKeyInput.VKKeys _left_key = SendKeyInput.VKKeys.KEY_A,
                                SendKeyInput.VKKeys _right_key = SendKeyInput.VKKeys.KEY_D,
                                SendKeyInput.VKKeys _forward_key = SendKeyInput.VKKeys.KEY_W)
        {
            player = _player;
            left_key = _left_key;
            right_key = _right_key;
            forward_key = _forward_key;
        }

        public struct Coords
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public static bool operator ==(Coords c1, Coords c2)
            {
                return c1.Equals(c2);
            }

            public static bool operator !=(Coords c1, Coords c2)
            {
                return !c1.Equals(c2);
            }

            public override int GetHashCode()
            {
                return string.Format("{0}_{1}_{2}", X, Y, Z).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Returns the distance to a given position
        /// </summary>
        /// <param name="X">X</param>
        /// <param name="Y">Y</param>
        /// <param name="Z">Z</param>
        /// <returns>Distance</returns>
        private double DistanceTo(double X, double Y, double Z)
        {
            player.refresh();
            return
                Math.Sqrt(Math.Pow((player.structure.X - X), 2) + Math.Pow((player.structure.Y - Y), 2) +
                          Math.Pow((Z - player.structure.Z), 2));
        }

        /// <summary>
        /// Converts -Pi to Pi radian to 0 to 2Pi degree
        /// </summary>
        /// <param name="angle">Radian angle (-Pi to Pi)</param>
        /// <returns>Degree angle (0 to 2Pi)</returns>
        private float XIVRadianToDegree(float angle)
        {
            return (float) ((angle + Math.PI)*(180.0/Math.PI));
        }

        /// <summary>
        /// Returns the angle we should face in order to reach the Target
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        private float HeadingTo(float X, float Y)
        {
            X = X - player.structure.X;
            Y = Y - player.structure.Y;
            double H = -Math.Atan2(Y, X);
            H = XIVRadianToDegree((float) H) + 90;
            return (float) H%360;
        }

        /// <summary>
        /// Computes the difference in Heading between player and target
        /// </summary>
        /// <param name="pHeading">Player Heading</param>
        /// <param name="tHeading">Target Heading</param>
        /// <returns>Difference in heading (degrees)</returns>
        private double HeadingError(double pHeading, double tHeading)
        {
            double diff = tHeading - pHeading;
            if (diff < 0)
                diff = Math.Abs(diff);
            return diff;
        }

        /// <summary>
        /// Returns the key to hit to shortest direction to go to in order to face the target.
        /// Yes, this is ugly.
        /// </summary>
        /// <param name="tHeading">Target heading</param>
        /// <param name="pHeading">Player heading</param>
        /// <returns>Key to press (left or right)</returns>
        private SendKeyInput.VKKeys WhereToHeadto(double tHeading, double pHeading)
        {
            if (pHeading < tHeading)
                {
                    if (tHeading - pHeading < 180)
                        return left_key;
                    else
                        return right_key;
                }
            if (pHeading > tHeading)
                {
                    if (pHeading - tHeading < 180)
                        return right_key;
                    else
                        return left_key;
                }
            return right_key;
        }

        /// <summary>
        /// Heads to a given coordinate
        /// </summary>
        /// <param name="X">X</param>
        /// <param name="Y">Y</param>
        /// <param name="Z">Z</param>
        public void GoToPos(float X, float Y, float Z)
        {
            bool IsRunning = false;
            float DistanceTolerance = 5f;
            double Heading = 0.0f;
            double PlayerHeading = 0.0f;
            double Herror = 0.0f;
            uint HeadingTolerance = 20;
            while (DistanceTo(X, Y, Z) > DistanceTolerance)
                {
                    player.refresh();
                    Heading = HeadingTo(X, Y);
                    PlayerHeading = XIVRadianToDegree(player.structure.heading);
                    Herror = HeadingError(PlayerHeading, Heading);
                    while (Herror > HeadingTolerance)
                        {
                            SendKeyInput.VKKeys key = WhereToHeadto(Heading, PlayerHeading);
                            ski.SendKeyPress(key);
                            player.refresh();
                            Heading = HeadingTo(X, Y);
                            PlayerHeading = XIVRadianToDegree(player.structure.heading);
                            Herror = HeadingError(PlayerHeading, Heading);
                        }
                    if (!IsRunning)
                        {
                            IsRunning = true;
                            ski.SendKeyPress(forward_key, false);
                        }
                }
            ski.SendKeyPress(forward_key, true);
        }

        /// <summary>
        /// Starts recording a list of coords to be used later as a waypoint.
        /// </summary>
        /// <param name="filename">XML to save waypoint to</param>
        /// <param name="delay">Optionnal delay between points</param>
        internal void _startRecordingWaypoint(string filename = null, int delay = 400)
        {
            Coords last = new Coords
                {
                    X = 0,
                    Y = 0,
                    Z = 0,
                };
            while (!_stop)
                {
                    Thread.Sleep(delay);
                    lock (coords_list)
                        {
                            if (coords_list.Count > 0)
                                last = coords_list.Last();
                            player.refresh();
                            Coords new_pos = new Coords
                                {
                                    X = player.structure.X,
                                    Y = player.structure.Y,
                                    Z = player.structure.Z,
                                };
                            if (new_pos != last)
                                coords_list.Add(new_pos);
                        }
                }
            if (filename != null)
                serializer.Serialize(filename, coords_list);
        }

        /// <summary>
        /// Starts a thread recording waypoint.
        /// </summary>
        /// <param name="filename">Filename to save waypoint to.</param>
        /// <param name="delay">Delay in ms between coordinates (default: 400)</param>
        public void startRecordingCoordinates(string filename = null, int delay = 400)
        {
            var t = new Thread(() => _startRecordingWaypoint(filename, delay));
            t.Start();
        }

        /// <summary>
        /// Stops the recording of Waypoint and return 
        /// the list of coordinates if you wish to process them.
        /// The waypoint is saved to file regardless.
        /// </summary>
        /// <returns>List of coordinates</returns>
        public List<Coords> stopRecordingWaypoint()
        {
            _stop = true;
            return coords_list;
        }

        /// <summary>
        /// Plays a pre-recorded waypoint file (XML) or a list of coordinates.
        /// It cannot handles both.
        /// </summary>
        /// <param name="filename">File to read waypoint from</param>
        /// <param name="list">List of coordinates</param>
        internal void _playWaypoint(string filename, List<Coords> list)
        {
            if (list == null && filename == null)
                return;
            if (list != null)
                coords_list = list.ToList();
            if (filename != null)
                coords_list = serializer.Deserialize<List<Coords>>(filename);
            foreach (Coords coord in coords_list)
                GoToPos(coord.X, coord.Y, coord.Z);
        }

        /// <summary>
        /// Plays a pre-recorded waypoint file (XML) or a list of coordinates.
        /// It cannot handles both.
        /// </summary>
        /// <param name="filename">File to read waypoint from</param>
        /// <param name="list">List of coordinates</param>
        public void playWaypoint(string filename = null, List<Coords> list = null)
        {
            var t = new Thread(() => _playWaypoint(filename, list));
            t.Start();
        }
    }

    public partial class FFXIVLIB
    {
        /// <summary>
        /// Returns a MovementHelper instance ready for work.
        /// </summary>
        /// <param name="_left_key">Left key (default: A)</param>
        /// <param name="_right_key">Right key (default: D)</param>
        /// <param name="_forward_key">Forward key (default: W)</param>
        /// <returns>MovementHelper instance</returns>
        public MovementHelper getMovementHelper(SendKeyInput.VKKeys _left_key = SendKeyInput.VKKeys.KEY_A,
                                                SendKeyInput.VKKeys _right_key = SendKeyInput.VKKeys.KEY_D,
                                                SendKeyInput.VKKeys _forward_key = SendKeyInput.VKKeys.KEY_W)
        {
            return new MovementHelper(getEntityInfo(0), _left_key, _right_key, _forward_key);
        }
    }
}