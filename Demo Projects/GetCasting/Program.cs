using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ffxivlib;

namespace GetCasting
{
    class Program
    {
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            while (true)
                {
                    Entity myself = instance.GetEntityById(0);
                    if (myself.IsCasting)
                        {
                            Console.WriteLine("{0} is casting {1} {2}%",
                                myself.Name,
                                ResourceParser.GetActionName(myself.CastingSpellId),
                                myself.CastingPercentage);
                        }
                    else
                        Console.WriteLine("{0} is not casting", myself.Name);
                    Thread.Sleep(300);
                }
        }
    }
}
