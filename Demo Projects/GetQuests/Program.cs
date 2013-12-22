using System;
using ffxivlib;

namespace GetQuests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            foreach (int quest in instance.GetQuests())
                Console.WriteLine(ResourceParser.GetQuestName(quest));
            Console.ReadLine();
        }
    }
}