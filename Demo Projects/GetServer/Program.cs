using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ffxivlib;
namespace GetServer
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            while (true)
                {
                    Console.WriteLine(instance.GetServerName());
                    Player p = instance.GetPlayerInfo();
                    Console.WriteLine("{0} - {1}", 
                        ResourceParser.GetZoneName(p.Zone), 
                        ResourceParser.GetZoneName(p.Subzone));
                    Thread.Sleep(1000);
                }
        }
    }
}
