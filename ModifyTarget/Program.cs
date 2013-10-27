using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ffxivlib;

namespace ModifyTarget
{
    class Program
    {
        /// <summary>
        /// Changes current target to yourself.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            // Retrieve our own character
            Entity e = instance.getEntityInfo(0);
            Target t = instance.getTargets();
            t.modify<int>("CurrentTarget", (int)e.address);
        }
    }
}
