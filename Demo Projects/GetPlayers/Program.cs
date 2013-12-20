using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ffxivlib;

namespace GetPlayers
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            var nodeList = instance.GetEntityByType(TYPE.Player);
            foreach (Entity e in nodeList)
            {
                Console.WriteLine("{0} {1}", e.Name, ResourceParser.GetJobShortname(e.Job));
            }
            Console.ReadLine();
        }
    }
}
