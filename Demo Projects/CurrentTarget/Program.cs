using System;
using System.Threading;
using ffxivlib;

namespace CurrentTarget
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            while (true)
                {
                    Entity currentTarget = instance.GetCurrentTarget();
                    Entity mouseoverTarget = instance.GetMouseoverTarget();
                    Entity myself = instance.GetEntityInfo(0);
                    if (currentTarget != null)
                        {
                        //Distance as represented in game
                            Console.WriteLine("Current target => {0} : {1}/{2} HP distance: {3} yalms",
                                              currentTarget.Name, currentTarget.CurrentHP,
                                              currentTarget.MaxHP, currentTarget.Distance);
                        }
                    if (mouseoverTarget != null)
                        {
                        //Distance calculated
                            Console.WriteLine("Mouseover target => {0} : {1}/{2} HP distance: {3} float",
                                              mouseoverTarget.Name, mouseoverTarget.CurrentHP,
                                              mouseoverTarget.MaxHP, mouseoverTarget.GetDistanceTo(myself));
                        }
                    Thread.Sleep(1000);
                }
// ReSharper disable once FunctionNeverReturns
        }
    }
}