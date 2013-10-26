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
                if (currentTarget != null)
                {
                    Console.WriteLine("Current target => {0} : {1}/{2} HP", currentTarget.structure.name, currentTarget.structure.cHP, currentTarget.structure.mHP);
                }
                if (mouseoverTarget != null)
                {
                    Console.WriteLine("Mouseover target => {0} : {1}/{2} HP", mouseoverTarget.structure.name, mouseoverTarget.structure.cHP, mouseoverTarget.structure.mHP);
                }
                Thread.Sleep(1000);
            }
         }
    }
}
