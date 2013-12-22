using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ffxivlib;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            int iterations = 100;
            while (true)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    for (int i = 0; i < iterations; i++)
                        {
                            instance.GetEntityByType(TYPE.Player);
                        }
                    sw.Stop();
                    Console.WriteLine((sw.ElapsedMilliseconds/iterations).ToString());
                }
        }
    }
}
