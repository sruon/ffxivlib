using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Action : BaseObject<Action.ACTIONINFO>
    {
        #region Constructor

        public Action(ACTIONINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Properties

        public int Unk_1 { get; set; }

        public int Unk_2 { get; set; }

        public int Id { get; set; }

        public int Unk_3 { get; set; }

        public int PercentUntilReady { get; set; }

        public bool IsReady { get; set; }

        public int Cost { get; set; }

        public int Unk_4 { get; set; }

        #endregion

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ACTIONINFO
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x4)] public int Unk_1;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8)] public int Unk_2;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xC)] public int Id;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x10)] public int Unk_3;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x14)] public int PercentUntilReady;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x18)] public bool IsReady;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x1C)] public int Cost;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x20)] public int Unk_4;
        };

        #endregion
    }
}