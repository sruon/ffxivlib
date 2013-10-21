using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ffxivlib;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            Entity.ENTITYINFO info = instance.getEntityInfo(2);

            Console.WriteLine(info.name);
            Console.ReadLine();
        }
    }
}
