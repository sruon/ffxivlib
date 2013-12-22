using System;
using System.Collections.Generic;
using System.Linq;
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
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x3C)] public int QuestID;

            /// <summary>
            /// Padding to make sure our struct are the same size as XIV ones, allows me to use Marshal.SizeOf
            /// </summary>
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x3E)] public short Padding;
        };

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct INVENTORYCONTAINER
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x0)] public int Pointer;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x4)] public int Header;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8)] public int Count;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x14)] public int Padding;
        }

        /// <summary>
        /// Structure holding all the pointers to different subarrays.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct INVENTORY
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)] [FieldOffset(0x0)] public INVENTORYCONTAINER[]
                Containers;
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

        /// <summary>
        /// Position of the specific inventory in the master array
        /// </summary>
        private enum Type : int
        {
            INVENTORY_1 = 0,
            INVENTORY_2 = 1,
            INVENTORY_3 = 2,
            INVENTORY_4 = 3,
            CURRENT_EQ = 4,
            EXTRA_EQ = 5,
            CRYSTALS = 6,
            QUESTS_KI = 9,
            AC_MH = 29,
            AC_OH = 30,
            AC_HEAD = 31,
            AC_BODY = 32,
            AC_HANDS = 33,
            AC_BELT = 34,
            AC_LEGS = 35,
            AC_FEET = 36,
            AC_EARRINGS = 37,
            AC_NECK = 38,
            AC_WRISTS = 39,
            AC_RINGS = 40,
            AC_SOULS = 41
        }

        #region Private methods

        /// <summary>
        /// Builds our list out of our pointers.
        /// </summary>
        /// <param name="ptr">First pointer to subarray</param>
        /// <param name="count">Number of pointers to process</param>
        /// <param name="clean"></param>
        /// <returns>Final list</returns>
        private InventoryBuilder BuildSubList(INVENTORYCONTAINER inventorycontainer, bool clean = true)
        {
            var ic = new InventoryBuilder((IntPtr) inventorycontainer.Pointer, inventorycontainer.Count, clean);
            return ic;
        }

        /// <summary>
        /// Builds our list out of our pointers.
        /// </summary>
        /// <param name="ptr">First pointer to subarray</param>
        /// <param name="count">Number of pointers to process</param>
        /// <param name="clean"></param>
        /// <returns>Final list</returns>
        private InventoryBuilder BuildSubList(bool cleaned, params INVENTORYCONTAINER[] inventorycontainer)
        {
            var ib = new InventoryBuilder();
            foreach (INVENTORYCONTAINER container in inventorycontainer)
                {
                    var ic = new InventoryBuilder((IntPtr) container.Pointer, container.Count, cleaned);
                    ib = ib + ic;
                }
            return ib;
        }

        internal List<ITEM> GetSelfInventory()
        {
            InventoryBuilder ic = BuildSubList(true, Structure.Containers[(int) Type.INVENTORY_1],
                Structure.Containers[(int) Type.INVENTORY_2], Structure.Containers[(int) Type.INVENTORY_2],
                Structure.Containers[(int) Type.INVENTORY_3], Structure.Containers[(int) Type.INVENTORY_4],
                Structure.Containers[(int) Type.CRYSTALS], Structure.Containers[(int) Type.EXTRA_EQ],
                Structure.Containers[(int) Type.CURRENT_EQ], Structure.Containers[(int) Type.QUESTS_KI]
                );
            return ic.Items;
        }

        internal List<ITEM> GetCurrentEquipment()
        {
            return BuildSubList(Structure.Containers[(int) Type.CURRENT_EQ], false).Items;
        }

        internal List<ITEM> GetQuests()
        {
            return BuildSubList(Structure.Containers[(int) Type.QUESTS_KI], false).Items;
        }

        internal List<ITEM> GetRetainerInventory()
        {
            throw new NotImplementedException();
        }

        internal List<ITEM> GetArmoryChest()
        {
            InventoryBuilder ic = BuildSubList(true, Structure.Containers[(int) Type.AC_MH],
                Structure.Containers[(int) Type.AC_OH], Structure.Containers[(int) Type.AC_HEAD],
                Structure.Containers[(int) Type.AC_BODY], Structure.Containers[(int) Type.AC_HANDS],
                Structure.Containers[(int) Type.AC_BELT], Structure.Containers[(int) Type.AC_LEGS],
                Structure.Containers[(int) Type.AC_FEET], Structure.Containers[(int) Type.AC_EARRINGS],
                Structure.Containers[(int) Type.AC_NECK], Structure.Containers[(int) Type.AC_RINGS],
                Structure.Containers[(int) Type.AC_WRISTS], Structure.Containers[(int) Type.AC_SOULS]);
            return ic.Items;
        }

        internal List<ITEM> GetCompanyInventory()
        {
            throw new NotImplementedException();
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
            IntPtr pointer = _mr.ResolvePointerPath(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetSelfInventory();
        }

        /// <summary>
        /// This returns your currently equipped items. See EQUIP_POS enum.
        /// </summary>
        /// <returns>List of items</returns>
        public List<Inventory.ITEM> GetCurrentEquipment()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.INVENTORYPTR);
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
            IntPtr pointer = _mr.ResolvePointerPath(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetRetainerInventory();
        }

        /// <summary>
        /// This returns your whole Armory Chest.
        /// </summary>
        /// <returns>List of items</returns>
        public List<Inventory.ITEM> GetArmoryChest()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetArmoryChest();
        }

        /// <summary>
        /// This returns your Free Company inventory, extra inventory (currency, crystals)
        /// This only works while checking the Free Company chest.
        /// </summary>
        /// <returns>List of items</returns>
        public List<Inventory.ITEM> GetCompanyInventory()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            return i.GetCompanyInventory();
        }

        /// <summary>
        /// Retrieves current quests
        /// </summary>
        /// <returns>List of quests ID</returns>
        public IEnumerable<int> GetQuests()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.INVENTORYPTR);
            var i = new Inventory(_mr.CreateStructFromAddress<Inventory.INVENTORY>(pointer), pointer);
            var quests = i.GetQuests();
            List<int> questsIds = new List<int>();
            foreach (Inventory.ITEM quest in quests)
                {
                    if (quest.QuestID != 0) // Key items have ID 0
                        {
                            questsIds.Add(quest.QuestID);
                        }
                }
            return questsIds.Distinct();
        }

        #endregion
    }
}