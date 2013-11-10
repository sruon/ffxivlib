using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ffxivlib;

namespace GetInventory
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
            Inventory.InventoryContainer ic = instance.getSelfInventory();
            ic = instance.getArmoryChest();
            ic = instance.getCompanyInventory();
            ic = instance.getRetainerInventory();
            ic = instance.getCurrentEquipment();
        }
    }
}
