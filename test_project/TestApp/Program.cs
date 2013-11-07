using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ffxivlib;
using System.Threading;

namespace CurrentTarget
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            while (true)
            {
                Entity currentTarget = instance.getCurrentTarget();
                Entity mouseoverTarget = instance.getMouseoverTarget();
                Entity myself = instance.getEntityInfo(0);
                if (currentTarget != null)
                {
                    Console.WriteLine("Current target => {0} : {1}/{2} HP distance: {3} yalms", currentTarget.structure.name, currentTarget.structure.cHP, currentTarget.structure.mHP, currentTarget.structure.distance);
                }
                if (mouseoverTarget != null)
                {
                    Console.WriteLine("Mouseover target => {0} : {1}/{2} HP distance: {3} float", mouseoverTarget.structure.name, mouseoverTarget.structure.cHP, mouseoverTarget.structure.mHP, mouseoverTarget.getDistanceTo(myself));
                }
                Thread.Sleep(1000);
            }
         }
    }
}
