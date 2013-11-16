using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ffxivlib;

namespace GetGatheringNodes
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            var node_list = instance.getEntityByType(TYPE.Gathering);
            foreach (Entity e in node_list)
                {
                    Console.WriteLine("{0} {1}", e.structure.name, e.structure.GatheringStatus);
                }
        }
    }
}
