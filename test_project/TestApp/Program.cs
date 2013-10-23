using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ffxivlib;
using System.Threading;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            Entity info = instance.getEntityInfo(0);
            PartyMember pmember = instance.getPartyMemberInfo(0);
            Player p = instance.getPlayerInfo();
            info.modify<byte>("icon", 12);
            info = instance.getEntityInfo(0);
            Console.ReadLine();
        }
    }
}
