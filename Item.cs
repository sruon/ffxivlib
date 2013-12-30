using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ffxivlib
{
    public class Item : BaseObject<Item.ITEMINFO>
    {
        #region Constructor

        public Item(ITEMINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Fields

        public uint Id { get; set; }

        public uint Amount { get; set; }

        public short Spiritbond { get; set; }

        public short Durability { get; set; }

        public int Materia_unk1 { get; set; }

        public byte Materia_unk2 { get; set; }

        public int QuestID { get; set; }

        #endregion

        #region Unmanaged structure

        /// <summary>
        /// Structure representing an item
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ITEMINFO
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8)] public uint Id;
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

        #endregion
    }
}
