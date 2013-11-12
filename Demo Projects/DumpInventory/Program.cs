using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            List<Inventory.ITEM> items = instance.getSelfInventory();
            // Just timing how long it takes
            Stopwatch objSW = new Stopwatch();
            objSW.Start();
            foreach (Inventory.ITEM item in items)
                {
                    Console.WriteLine("{0} x{1}", ResourceParser.getItemName(item), item.Amount);
                }
            objSW.Stop();
            // You can check the stopwatch here and note the time.
            Console.ReadLine();
            objSW.Reset();
            objSW.Start();
            // In cache values
            foreach (Inventory.ITEM item in items)
            {
                Console.WriteLine("{0} x{1}", ResourceParser.getItemName(item), item.Amount);
            }
            objSW.Stop();
            // You can check the stopwatch value here and compare. Profit
            Console.ReadLine();
        }
    }
}
