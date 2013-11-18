using System;
using System.Threading;
using ffxivlib;

namespace PlayerHeading
{
    internal class Program
    {
        /// <summary>
        /// Quick program to test my navigation stuff
        /// http://www.ffevo.net/wiki/index.php/FFACETools
        /// Credits goes to cpirie
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            Entity player = instance.GetEntityInfo(0);
            while (true)
                {
                    string sResult = "Heading: ";
                    double degrees = player.Structure.Heading*(180/Math.PI) + 180;
                    if (degrees > 360)
                        degrees -= 360;
                    else if (degrees < 0)
                        degrees += 360;
                    sResult += Math.Floor(degrees) + "° ";

                    if (337 < degrees || 23 >= degrees)
                        sResult += "(N)";
                    else if (23 < degrees && 68 >= degrees)
                        sResult += "(NW)";
                    else if (68 < degrees && 113 >= degrees)
                        sResult += "(W)";
                    else if (113 < degrees && 158 >= degrees)
                        sResult += "(SW)";
                    else if (158 < degrees && 203 >= degrees)
                        sResult += "(S)";
                    else if (203 < degrees && 248 >= degrees)
                        sResult += "(SE)";
                    else if (248 < degrees && 293 >= degrees)
                        sResult += "(E)";
                    else if (293 < degrees && 337 >= degrees)
                        sResult += "(NE)";
                    Console.WriteLine("Heading: {0} Deg: {1}, X: {2}, Y: {3}", player.Structure.Heading.ToString(),
                                      sResult, player.Structure.X.ToString(), player.Structure.Y.ToString());
                    player.Refresh();
                    Thread.Sleep(1000);
                }
// ReSharper disable once FunctionNeverReturns
        }
    }
}