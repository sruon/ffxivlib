using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ffxivlib;

namespace GetPlayers
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            while (true)
                {
                    var nodeList = instance.GetEntityByType(TYPE.Player);
                    foreach (Entity e in nodeList)
                        {
                            Console.WriteLine("{0} {1}{2}", e.Name, ResourceParser.GetJobShortname(e.Job), e.Level);
                        }
                    Thread.Sleep(2000);
                    Console.Clear();
                }
        }
    }
}
