using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class PartyMember : IContainer<PartyMember, PartyMember.PARTYMEMBERINFO>
    {
        public PartyMember(PARTYMEMBERINFO _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct PARTYMEMBERINFO
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0)] public int PlayerID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] [FieldOffset(0x10)] public string name;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x51)] public JOB job;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x53)] public byte level;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x58)] public int cHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x5C)] public int mHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x60)] public int cMP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x62)] public int mMP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x64)] public short cTP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x66)] public short cGP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x68)] public short mGP;
            //[MarshalAs(UnmanagedType.LPArray, SizeConst=15)]
            //[FieldOffset(0x70)]
            //public STATUSEFFECT[] buffs;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x1F0)] public float X;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x1F4)] public float Z;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x1F8)] public float Y;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x38C)] public int padding;
        };

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct STATUSEFFECT
        {
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0)] public byte buff;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8)] public int provider;
        };
    }
}