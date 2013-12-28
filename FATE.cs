using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class FATE : BaseObject<FATE.FATEINFO>
    {
        #region Constructor

        public FATE(FATEINFO structure, IntPtr address)
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
        public struct FATEINFO
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x18)] public int Id;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xAE)] public string Name;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x330)] public float X;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x334)] public float Z;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x338)] public float Y;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x344)] public float Radius;
        };

        #endregion
    }
}