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
            Entity info = instance.getEntityInfo(0);
            info.modify<byte>("icon", 1);
            info = instance.getEntityInfo(0);
            Console.ReadLine();
        }
    }
}
