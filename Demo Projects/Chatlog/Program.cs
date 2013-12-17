using System;
using System.Collections.Generic;
using System.Threading;
using ffxivlib;

namespace ChatlogTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB(0x1A98);
            Chatlog c = instance.GetChatlog();
            while (true)
                {
                    if (c.IsNewLine())
                        {
                            List<Chatlog.Entry> test = c.GetChatLogLines();
                            if (test.Count > 0)
                                Console.WriteLine("{0} new log lines", test.Count);
                            foreach (Chatlog.Entry line in test)
                                {
                                    Console.WriteLine("{0}[{1}] -> {2}", line.Timestamp, line.Code, line.Text);
                                    if (line.RawModified != null)
                                        Console.WriteLine(BitConverter.ToString(line.RawModified));
                                }
                        }
                    Thread.Sleep(300);
                }
// ReSharper disable once FunctionNeverReturns
        }
    }
}