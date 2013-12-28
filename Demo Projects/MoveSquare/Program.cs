using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ffxivlib;

namespace MoveSquare
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            Movement m = instance.GetMovement();
            m.MoveForward();
            Thread.Sleep(3000);
            m.StopMoving();
            m.HeadRight();
            Thread.Sleep(1500);
            m.StopMoving();
            m.MoveForward();
            Thread.Sleep(3000);
            m.StopMoving();
        }
    }
}
