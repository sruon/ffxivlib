using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Buff : BaseObject<Buff.BUFFINFO>
    {
         #region Constructor

        public Buff(BUFFINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Properties

        public short Id { get; set; }

        public float TimeLeft { get; set; }

        public int BuffProvider { get; set; }

        #endregion

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct BUFFINFO
        {
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0)] public short Id;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x4)] public float TimeLeft;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8)] public int BuffProvider;
        };

        #endregion

    }
}