using ffxivlib;

namespace ModifyTarget
{
    internal class Program
    {
        /// <summary>
        /// Changes current target to yourself.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            // Retrieve our own character
            Entity e = instance.getEntityInfo(0);
            Target t = instance.getTargets();
            t.modify("CurrentTarget", (int) e.address);
        }
    }
}