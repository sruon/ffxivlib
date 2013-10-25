using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ffxivlib;
using System.Threading;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            while (true)
            {
                try
                {
                    Entity target = instance.getCurrentTarget();
                    Console.WriteLine("{0} : {1}/{2} HP", target.structure.name, target.structure.cHP, target.structure.mHP);
                }
                catch
                {
                }
                Thread.Sleep(3000);
            }
         }
    }
}
