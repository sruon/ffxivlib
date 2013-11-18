using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Inventory : BaseObject<Inventory.INVENTORY>
    {
        #region Constructor

        public Inventory(INVENTORY structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Fields

        private readonly MemoryReader _mr = MemoryReader.GetInstance();

        #endregion

        #region Unmanaged structure

        /// <summary>
        /// Structure representing an item
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ITEM
        {
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x8)]
            public uint ItemID;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x0C)]
            public uint Amount;

            /// <summary>
            /// 10000 = fully spiritbond
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x10)]
            public short Spiritbond;

            /// <summary>
            /// 30000 = fully repaired
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x12)]
            public short Durability;

            /// <summary>
            /// Related to materia slots, but no idea what.
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x18)]
            public int Materia_unk1;

            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x1E)]
            public byte Materia_unk2;

            /// <summary>
            /// I am deeply confused about this one, if item is related to a quest this is equal to the QuestID.
            /// BUT, if you accept a quest, there will still be instances in the keyitem container of said questid with empty items. Why SE?
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x3C)]
            public uint QuestID;

            /// <summary>
            /// Padding to make sure our struct are the same size as XIV ones, allows me to use Marshal.SizeOf
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x3E)]
            public short Padding;
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

        #endregion

        #region InventoryBuilder

        /// <summary>
        /// Basic container for our list of items.
        /// </summary>
        public class InventoryBuilder
        {
            public List<ITEM> Items { get; set; }

            /// <summary>
            /// Constructor building the list of items out of initial pointer and count.
            /// </summary>
            /// <param name="pointer">Subarray pointer</param>
            /// <param name="count">Subarray max elements</param>
            /// <param name="clean">Should we ignore empty objects? Useful for currently equipped items.</param>
            internal InventoryBuilder(IntPtr pointer, int count, bool clean)
            {
                MemoryReader mr = MemoryReader.GetInstance();
                Items = new List<ITEM>();
                for (int i = 0; i < count; i++)
                    {
                        var currentItem = mr.CreateStructFromAddress<ITEM>(pointer + (i*Marshal.SizeOf(typeof (ITEM))));
                        if (currentItem.ItemID != 0 || currentItem.QuestID != 0 || !clean)
                            Items.Add(currentItem);
                    }
            }

            internal InventoryBuilder()
            {
                var mr = MemoryReader.GetInstance();
            }

            /// <summary>
            /// Merges two lists
            /// </summary>
            /// <param name="ic1">First list</param>
            /// <param name="ic2">Second list</param>
            /// <returns>New merged list</returns>
            public static InventoryBuilder operator +(InventoryBuilder ic1, InventoryBuilder ic2)
            {
                var ic = new InventoryBuilder {Items = new List<ITEM>()};
                if (ic1.Items != null)
                    ic.Items.AddRange(ic1.Items);
                if (ic2.Items != null)
                    ic.Items.AddRange(ic2.Items);
                return ic;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Builds our list out of our pointers.
        /// </summary>
        /// <param name="ptr">First pointer to subarray</param>
        /// <param name="count">Number of pointers to process</param>
        /// <param name="clean"></param>
        /// <returns>Final list</returns>
        private InventoryBuilder BuildSubList(IntPtr ptr, int count, bool clean = true)
        {
            var ic = new InventoryBuilder();
            for (int i = 0; i < count; i++)
                {
                    IntPtr icPTR = _mr.ResolvePointer(ptr + (i * Constants.INVENTORY_PTR_OFFSET));
                    // Count of container is at 0xC.
                    var icCount = (int)_mr.ResolvePointer(ptr + (i * Constants.INVENTORY_PTR_OFFSET) + 0xC);
                    ic = ic + new InventoryBuilder(icPTR, icCount, clean);
                }
            return ic;
        }

        internal List<ITEM> GetSelfInventory()
        {
            InventoryBuilder ic = BuildSubList((IntPtr) Structure.SelfInventory, Constants.SELF_INVENTORY_SIZE);
            ic = ic + BuildSubList((IntPtr) Structure.SelfExtra, Constants.SELF_EXTRA_SIZE);
            ic = ic + BuildSubList((IntPtr) Structure.CurrentEquipment, Constants.CURRENT_EQUIPMENT_SIZE);
            return ic.Items;
        }

        internal List<ITEM> GetCurrentEquipment()
        {
            return BuildSubList((IntPtr) Structure.CurrentEquipment, Constants.CURRENT_EQUIPMENT_SIZE, false).Items;
        }

        internal List<ITEM> GetRetainerInventory()
        {
            InventoryBuilder ic = BuildSubList((IntPtr) Structure.RetainerInventory, Constants.RETAINER_INVENTORY_SIZE);
            ic = ic + BuildSubList((IntPtr) Structure.RetainerExtra, Constants.RETAINER_EXTRA_SIZE);
            return ic.Items;
        }

        internal List<ITEM> GetArmoryChest()
        {
            InventoryBuilder ic = BuildSubList((IntPtr) Structure.ArmoryChestMH, Constants.ARMORY_CHEST_MH_SIZE);
            ic = ic + BuildSubList((IntPtr) Structure.ArmoryChest, Constants.ARMORY_CHEST_SIZE);
            return ic.Items;
        }

        internal List<ITEM> GetCompanyInventory()
        {
            InventoryBuilder ic = BuildSubList((IntPtr) Structure.CompanyInventory, Constants.COMPANY_INVENTORY_SIZE);
            ic = ic + BuildSubList((IntPtr) Structure.CompanyExtra, Constants.COMPANY_EXTRA_SIZE);
            return ic.Items;
        }

        #endregion
    }

    public partial class FFXIVLIB
    {
        #region Public methods

        /// <summary>
        /// This returns your inventory, extra inventory (gil, crystals, tomes, seals), 
        /// key items, calamity salvager and currently equipped items
        /// </summary>
        /// <returns>List of items</returns>
        public List<Inventory.ITEM> GetSelfInventory()
        {
            IntPtr pointer = _mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetSelfInventory();
        }

        /// <summary>
        /// This returns your currently equipped items. See EQUIP_POS enum.
        /// </summary>
        /// <returns>List of items</returns>
        public List<Inventory.ITEM> GetCurrentEquipment()
        {
            IntPtr pointer = _mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetCurrentEquipment();
        }

        /// <summary>
        /// This returns your retainer inventory, extra inventory (gil, crystals) and what is up for sale.
        /// This only works while checking a retainer.
        /// </summary>
        /// <returns>List of items</returns>
        public List<Inventory.ITEM> GetRetainerInventory()
        {
            IntPtr pointer = _mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetRetainerInventory();
        }

        /// <summary>
        /// This returns your whole Armory Chest.
        /// </summary>
        /// <returns>List of items</returns>
        public List<Inventory.ITEM> GetArmoryChest()
        {
            IntPtr pointer = _mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetArmoryChest();
        }

        /// <summary>
        /// This returns your Free Company inventory, extra inventory (currency, crystals)
        /// This might only work while checking the Free Company chest.
        /// </summary>
        /// <returns>List of items</returns>
        public List<Inventory.ITEM> GetCompanyInventory()
        {
            IntPtr pointer = _mr.GetArrayStart(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetCompanyInventory();
        }

        #endregion
    }
}