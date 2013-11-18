using System.Collections.Generic;
using ffxivlib;

namespace GetInventory
{
    class Program
    {
        static void Main(string[] args)
        {
            FFXIVLIB instance = new FFXIVLIB();
// ReSharper disable once NotAccessedVariable
            List<Inventory.ITEM> ic = instance.GetSelfInventory();
// ReSharper disable RedundantAssignment
            ic = instance.GetArmoryChest();
            ic = instance.GetCompanyInventory();
            ic = instance.GetRetainerInventory();
            ic = instance.GetCurrentEquipment();
            // ReSharper restore RedundantAssignment
        }
    }
}
