using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ffxivlib;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB lib = new FFXIVLIB();
            lib.TestMR();
            Console.ReadLine();
        }
    }
}
