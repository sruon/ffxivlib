using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ffxivlib;

namespace ModifyIcon
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            // 0 is always your own character
            Entity e = instance.getEntityInfo(0);
            Console.WriteLine("Changing icon to Yoshi-P for player {0}", e.structure.name);
            e.modify<byte>("icon", (byte)ICON.Yoshida);
            Console.WriteLine("Done..");
            Console.ReadLine();
        }
    }
}
