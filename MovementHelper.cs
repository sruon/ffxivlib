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
        #region Constructor

        internal MovementHelper(Entity player,
                                SendKeyInput.VKKeys leftKey = SendKeyInput.VKKeys.KEY_A,
                                SendKeyInput.VKKeys rightKey = SendKeyInput.VKKeys.KEY_D,
                                SendKeyInput.VKKeys forwardKey = SendKeyInput.VKKeys.KEY_W)
        {
            _player = player;
            _leftKey = leftKey;
            _rightKey = rightKey;
            _forwardKey = forwardKey;
            _pause = false;
        }

        #endregion

        #region Fields

        private volatile bool _stop;
        private volatile bool _pause;
        private readonly Entity _player;
        private readonly SendKeyInput _ski = SendKeyInput.GetInstance();
        private List<Coords> _coordsList = new List<Coords>();
        private readonly Serializer _serializer = new Serializer();
        private readonly SendKeyInput.VKKeys _leftKey;
        private readonly SendKeyInput.VKKeys _rightKey;
        private readonly SendKeyInput.VKKeys _forwardKey;

        #endregion

        #region Coords

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

        #endregion

        #region Private methods

        /// <summary>
        /// Returns the distance to a given position
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <returns>Distance</returns>
        private double DistanceTo(double x, double y, double z)
        {
            _player.Refresh();
            return
                Math.Sqrt(Math.Pow((_player.Structure.X - x), 2) + Math.Pow((_player.Structure.Y - y), 2) +
                          Math.Pow((z - _player.Structure.Z), 2));
        }

        /// <summary>
        /// Converts -Pi to Pi radian to 0 to 2Pi degree
        /// </summary>
        /// <param name="angle">Radian angle (-Pi to Pi)</param>
        /// <returns>Degree angle (0 to 2Pi)</returns>
        private static float XIVRadianToDegree(float angle)
        {
            return (float) ((angle + Math.PI)*(180.0/Math.PI));
        }

        /// <summary>
        /// Returns the angle we should face in order to reach the Target
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private float HeadingTo(float x, float y)
        {
            x = x - _player.Structure.X;
            y = y - _player.Structure.Y;
            double h = -Math.Atan2(y, x);
            h = XIVRadianToDegree((float) h) + 90;
            return (float) h%360;
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
                        return _leftKey;
                    return _rightKey;
                }
            if (pHeading > tHeading)
                {
                    if (pHeading - tHeading < 180)
                        return _rightKey;
                    return _leftKey;
                }
            return _rightKey;
        }

        
        /// <summary>
        /// Starts recording a list of coords to be used later as a waypoint.
        /// </summary>
        /// <param name="filename">XML to save waypoint to</param>
        /// <param name="delay">Optionnal delay between points</param>
        internal void _startRecordingWaypoint(string filename = null, int delay = 400)
        {
            var last = new Coords
                {
                    X = 0,
                    Y = 0,
                    Z = 0,
                };
            while (!_stop)
                {
                    Thread.Sleep(delay);
                    lock (_coordsList)
                        {
                            if (_coordsList.Count > 0)
                                last = _coordsList.Last();
                            _player.Refresh();
                            var newPos = new Coords
                                {
                                    X = _player.Structure.X,
                                    Y = _player.Structure.Y,
                                    Z = _player.Structure.Z,
                                };
                            if (newPos != last)
                                _coordsList.Add(newPos);
                        }
                }
            if (filename != null)
                _serializer.Serialize(filename, _coordsList);
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
                _coordsList = list.ToList();
            if (filename != null)
                _coordsList = _serializer.Deserialize<List<Coords>>(filename);
            foreach (Coords coord in _coordsList)
                GoToPos(coord.X, coord.Y, coord.Z);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Heads to a given coordinate
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        public void GoToPos(float x, float y, float z)
        {
            bool isRunning = false;
            const float distanceTolerance = 5f;
            const uint headingTolerance = 20;
            while (DistanceTo(x, y, z) > distanceTolerance)
            {
                if (!_pause)
                {
                    _player.Refresh();
                    double heading = HeadingTo(x, y);
                    double playerHeading = XIVRadianToDegree(_player.Structure.Heading);
                    double herror = HeadingError(playerHeading, heading);
                    while (herror > headingTolerance)
                    {
                        SendKeyInput.VKKeys key = WhereToHeadto(heading, playerHeading);
                        _ski.SendKeyPress(key);
                        _player.Refresh();
                        heading = HeadingTo(x, y);
                        playerHeading = XIVRadianToDegree(_player.Structure.Heading);
                        herror = HeadingError(playerHeading, heading);
                    }
                    if (!isRunning)
                    {
                        isRunning = true;
                        _ski.SendKeyPress(_forwardKey, false);
                    }
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            _ski.SendKeyPress(_forwardKey);
        }

        /// <summary>
        /// Starts a thread recording waypoint.
        /// </summary>
        /// <param name="filename">Filename to save waypoint to.</param>
        /// <param name="delay">Delay in ms between coordinates (default: 400)</param>
        public void StartRecordingCoordinates(string filename = null, int delay = 400)
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
        public List<Coords> StopRecordingWaypoint()
        {
            _stop = true;
            return _coordsList;
        }

        /// <summary>
        /// Plays a pre-recorded waypoint file (XML) or a list of coordinates.
        /// It cannot handles both.
        /// </summary>
        /// <param name="filename">File to read waypoint from</param>
        /// <param name="list">List of coordinates</param>
        public void PlayWaypoint(string filename = null, List<Coords> list = null)
        {
            var t = new Thread(() => _playWaypoint(filename, list));
            t.Start();
        }

        /// <summary>
        /// This pauses the playWaypoint thread.
        /// Sets pause to true or false depending on current value.
        /// </summary>
        /// <returns>Value of pause after this call.</returns>
        public bool PauseWaypoint()
        {
            _pause = !_pause;
            return (_pause);
        }

        #endregion
    }

    public partial class FFXIVLIB
    {
        #region Public methods

        /// <summary>
        /// Returns a MovementHelper instance ready for work.
        /// </summary>
        /// <param name="leftKey">Left key (default: A)</param>
        /// <param name="rightKey">Right key (default: D)</param>
        /// <param name="forwardKey">Forward key (default: W)</param>
        /// <returns>MovementHelper instance</returns>
        public MovementHelper GetMovementHelper(SendKeyInput.VKKeys leftKey = SendKeyInput.VKKeys.KEY_A,
                                                SendKeyInput.VKKeys rightKey = SendKeyInput.VKKeys.KEY_D,
                                                SendKeyInput.VKKeys forwardKey = SendKeyInput.VKKeys.KEY_W)
        {
            return new MovementHelper(GetEntityInfo(0), leftKey, rightKey, forwardKey);
        }

        #endregion
    }
}