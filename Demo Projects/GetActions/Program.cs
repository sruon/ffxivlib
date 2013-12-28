using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ffxivlib;

namespace GetActions
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();

            foreach (var i in instance.GetActions())
            {
                Console.WriteLine("ID: {0} IsReady: {1} Cost: {2} PercentUntilReady: {3} Unk1: {4} Unk2: {5} Unk3: {6} Unk4: {7}",
                    i.Id,
                    i.IsReady,
                    i.Cost,
                    i.PercentUntilReady,
                    i.Unk_1,
                    i.Unk_2,
                    i.Unk_3,
                    i.Unk_4);
                Console.ReadLine();

            }
        }
    }
}
