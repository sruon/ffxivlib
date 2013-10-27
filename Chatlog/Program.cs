using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ffxivlib;
using System.Threading;

namespace Chatlog
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            ffxivlib.Chatlog c = instance.getChatlog();
            while (true)
            {
                List<string> test = c.getChatLogStrings();
                if (test.Count > 0)
                {
                    Console.WriteLine("{0} new log lines", test.Count.ToString());
                }
                Thread.Sleep(3000);
            }
            }
    }
}
