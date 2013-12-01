using System.Runtime.InteropServices;

namespace ffxivlib
{
    #region Unmanaged structure

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct BUFF
    {
        [MarshalAs(UnmanagedType.I2)] [FieldOffset(0)] public short BuffID;
        [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x4)] public float TimeLeft;
        [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8)] public int BuffProvider;
    };

    #endregion
}