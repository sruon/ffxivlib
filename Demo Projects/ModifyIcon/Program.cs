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
            Entity e = instance.GetEntityInfo(0);
            Console.WriteLine("Changing icon to Yoshi-P for player {0}", e.Structure.Name);
            e.Modify("Icon", (byte) ICON.Yoshida);
            Console.WriteLine("Done..");
            Console.ReadLine();
        }
    }
}