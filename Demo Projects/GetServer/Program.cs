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
                    Thread.Sleep(1000);
                }
        }
    }
}
