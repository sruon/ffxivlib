using System;
using System.Collections.Generic;
using System.Diagnostics;
using ffxivlib;

namespace DumpInventory
{
    class Program
    {
        /// <summary>
        /// Place all your resources in DumpInventory/bin/debug/myresourcefolder before running.
        /// Change folder or language if you wish to.
        /// Those changes HAVE TO BE DONE BEFORE INSTANTIATING FFXIVLIB
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Default folder is Resources, you may override it here
            Constants.ResourceParser.RESOURCES_FOLDER = "myresourcefolder";
            // ja, fr, de or en. Defaults to en if not set
            Constants.ResourceParser.RESOURCES_LANGUAGE = "de";
            FFXIVLIB instance = new FFXIVLIB();
            List<Inventory.ITEM> items = instance.GetSelfInventory();
            // Just timing how long it takes
            Stopwatch objSw = new Stopwatch();
            objSw.Start();
            foreach (Inventory.ITEM item in items)
                {
                    Console.WriteLine("{0} x{1}", ResourceParser.GetItemName(item), item.Amount);
                }
            objSw.Stop();
            // You can check the stopwatch here and note the time.
            Console.ReadLine();
            objSw.Reset();
            objSw.Start();
            // In cache values
            foreach (Inventory.ITEM item in items)
            {
                Console.WriteLine("{0} x{1}", ResourceParser.GetItemName(item), item.Amount);
            }
            objSw.Stop();
            // You can check the stopwatch value here and compare. Profit
            Console.ReadLine();
        }
    }
}
