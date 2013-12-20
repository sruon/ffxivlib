using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ffxivlib;

namespace GetGatheringNodes
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB(0xC18);
            var nodeList = instance.GetEntityByType(TYPE.Gathering);
            foreach (Entity e in nodeList)
            {
                Console.WriteLine("{0} {1}", e.Name, e.Structure.GatheringStatus);
            }
            Console.ReadLine();
        }
    }
}
