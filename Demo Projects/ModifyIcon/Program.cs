using System;
using ffxivlib;

namespace ModifyIcon
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            // 0 is always your own character
            Entity e = instance.getEntityInfo(0);
            Console.WriteLine("Changing icon to Yoshi-P for player {0}", e.structure.name);
            e.modify("icon", (byte) ICON.Yoshida);
            Console.WriteLine("Done..");
            Console.ReadLine();
        }
    }
}