using System;
using ffxivlib;

namespace GetGatheringNodes
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            var nodeList = instance.GetEntityByType(TYPE.Gathering);
            foreach (Entity e in nodeList)
                {
                    Console.WriteLine("{0} {1}", e.Name, e.Structure.GatheringStatus);
                }
            Console.ReadLine();
        }
    }
}
