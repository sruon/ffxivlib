using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Inventory : IContainer<Inventory, Inventory.INVENTORY>
    {
        private readonly MemoryReader mr = MemoryReader.getInstance();

        /// <summary>
        /// Structure representing an item
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ITEM
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8)] public uint ItemID;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x0C)] public uint Amount;

            /// <summary>
            /// 10000 = fully spiritbond
            /// </summary>
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x10)] public short Spiritbond;

            /// <summary>
            /// 30000 = fully repaired
            /// </summary>
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x12)] public short Durability;

            /// <summary>
            /// Related to materia slots, but no idea what.
            /// </summary>
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x18)] public int Materia_unk1;

            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x1E)] public byte Materia_unk2;

            /// <summary>
            /// I am deeply confused about this one, if item is related to a quest this is equal to the QuestID.
            /// BUT, if you accept a quest, there will still be instances in the keyitem container of said questid with empty items. Why SE?
            /// </summary>
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x3C)] public uint QuestID;

            /// <summary>
            /// Padding to make sure our struct are the same size as XIV ones, allows me to use Marshal.SizeOf
            /// </summary>
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x3E)] public short Padding;
        };

        /// <summary>
        /// Structure holding all the pointers to different subarrays.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct INVENTORY
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x0)] public int SelfInventory;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x4)] public int CurrentEquipment;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8)] public int SelfExtra;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xC)] public int RetainerInventory;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x14)] public int RetainerExtra;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x18)] public int ArmoryChestMH;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x1C)] public int ArmoryChest;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x20)] public int CompanyInventory;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x24)] public int CompanyExtra;
        }

        public Inventory(INVENTORY _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }

        /// <summary>
        /// Basic container for our list of items.
        /// </summary>
        public class InventoryBuilder
        {
            public List<ITEM> items { get; set; }
            private readonly MemoryReader mr = MemoryReader.getInstance();

            /// <summary>
            /// Constructor building the list of items out of initial pointer and count.
            /// </summary>
            /// <param name="pointer">Subarray pointer</param>
            /// <param name="count">Subarray max elements</param>
            /// <param name="clean">Should we ignore empty objects? Useful for currently equipped items.</param>
            internal InventoryBuilder(IntPtr pointer, int count, bool clean)
            {
                items = new List<ITEM>();
                for (int i = 0; i < count; i++)
                    {
                        ITEM currentItem = mr.CreateStructFromAddress<ITEM>(pointer + (i*Marshal.SizeOf(typeof (ITEM))));
                        if (currentItem.ItemID != 0 || currentItem.QuestID != 0 || !clean)
                            items.Add(currentItem);
                    }
            }

            internal InventoryBuilder() {}

            /// <summary>
            /// Merges two lists
            /// </summary>
            /// <param name="ic1">First list</param>
            /// <param name="ic2">Second list</param>
            /// <returns>New merged list</returns>
            public static InventoryBuilder operator +(InventoryBuilder ic1, InventoryBuilder ic2)
            {
                InventoryBuilder ic = new InventoryBuilder();
                ic.items = new List<ITEM>();
                if (ic1.items != null)
                    ic.items.AddRange(ic1.items);
                if (ic2.items != null)
                    ic.items.AddRange(ic2.items);
                return ic;
            }
        }

        /// <summary>
        /// Builds our list out of our pointers.
        /// </summary>
        /// <param name="ptr">First pointer to subarray</param>
        /// <param name="count">Number of pointers to process</param>
        /// <param name="offset">Offset between pointers</param>
        /// <returns>Final list</returns>
        private InventoryBuilder buildSubList(IntPtr ptr, int count, bool clean = true)
        {
            InventoryBuilder ic = new InventoryBuilder();
            for (int i = 0; i < count; i++)
                {
                    IntPtr ic_ptr = mr.ResolvePointer(ptr + (i * Constants.INVENTORY_PTR_OFFSET));
                    // Count of container is at 0xC.
                    int ic_count = (int)mr.ResolvePointer(ptr + (i * Constants.INVENTORY_PTR_OFFSET) + 0xC);
                    ic = ic + new InventoryBuilder(ic_ptr, ic_count, clean);
                }
            return ic;
        }

        internal List<ITEM> getSelfInventory()
        {
            InventoryBuilder ic = buildSubList((IntPtr) structure.SelfInventory, Constants.SELF_INVENTORY_SIZE);
            ic = ic + buildSubList((IntPtr) structure.SelfExtra, Constants.SELF_EXTRA_SIZE);
            ic = ic + buildSubList((IntPtr) structure.CurrentEquipment, Constants.CURRENT_EQUIPMENT_SIZE);
            return ic.items;
        }

        internal List<ITEM> getCurrentEquipment()
        {
            return buildSubList((IntPtr) structure.CurrentEquipment, Constants.CURRENT_EQUIPMENT_SIZE, false).items;
        }

        internal List<ITEM> getRetainerInventory()
        {
            InventoryBuilder ic = buildSubList((IntPtr) structure.RetainerInventory, Constants.RETAINER_INVENTORY_SIZE);
            ic = ic + buildSubList((IntPtr) structure.RetainerExtra, Constants.RETAINER_EXTRA_SIZE);
            return ic.items;
        }

        internal List<ITEM> getArmoryChest()
        {
            InventoryBuilder ic = buildSubList((IntPtr) structure.ArmoryChestMH, Constants.ARMORY_CHEST_MH_SIZE);
            ic = ic + buildSubList((IntPtr) structure.ArmoryChest, Constants.ARMORY_CHEST_SIZE);
            return ic.items;
        }

        internal List<ITEM> getCompanyInventory()
        {
            InventoryBuilder ic = buildSubList((IntPtr) structure.CompanyInventory, Constants.COMPANY_INVENTORY_SIZE);
            ic = ic + buildSubList((IntPtr) structure.CompanyExtra, Constants.COMPANY_EXTRA_SIZE);
            return ic.items;
        }
    }

    public partial class FFXIVLIB
    {
        /// <summary>
        /// This returns your inventory, extra inventory (gil, crystals, tomes, seals), 
        /// key items, calamity salvager and currently equipped items
        /// </summary>
        /// <returns>Requested Inventory</returns>
        public List<Inventory.ITEM> getSelfInventory()
        {
            IntPtr pointer = mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.getSelfInventory();
        }

        /// <summary>
        /// This returns your currently equipped items. See EQUIP_POS enum.
        /// </summary>
        /// <returns>Requested Inventory</returns>
        public List<Inventory.ITEM> getCurrentEquipment()
        {
            IntPtr pointer = mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.getCurrentEquipment();
        }

        /// <summary>
        /// This returns your retainer inventory, extra inventory (gil, crystals) and what is up for sale.
        /// This only works while checking a retainer.
        /// </summary>
        /// <returns>Requested Inventory</returns>
        public List<Inventory.ITEM> getRetainerInventory()
        {
            IntPtr pointer = mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.getRetainerInventory();
        }

        /// <summary>
        /// This returns your whole Armory Chest.
        /// </summary>
        /// <returns>Requested Inventory</returns>
        public List<Inventory.ITEM> getArmoryChest()
        {
            IntPtr pointer = mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.getArmoryChest();
        }

        /// <summary>
        /// This returns your Free Company inventory, extra inventory (currency, crystals)
        /// This might only work while checking the Free Company chest.
        /// </summary>
        /// <returns>Requested Inventory</returns>
        public List<Inventory.ITEM> getCompanyInventory()
        {
            IntPtr pointer = mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.getCompanyInventory();
        }
    }
}