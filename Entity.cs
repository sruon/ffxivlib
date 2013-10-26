using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ffxivlib
{

    public class Entity : IContainer<Entity, Entity.ENTITYINFO>
    {
        [StructLayout(LayoutKind.Explicit, Pack=1)]
        public struct ENTITYINFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            [FieldOffset(0x30)]
            public string name;
            // Not exactly PC ID...
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x74)]
            public int PCID;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x78)] 
            public int NPCID;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x8A)] 
            public TYPE MobType;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x8C)]
            public CURRENTTARGET currentTarget;
            [MarshalAs(UnmanagedType.R4)]
            [FieldOffset(0xA0)] 
            public float X;
            [MarshalAs(UnmanagedType.R4)]
            [FieldOffset(0xA4)]
            public float Z;
            [MarshalAs(UnmanagedType.R4)]
            [FieldOffset(0xA8)]
            public float Y;
            [MarshalAs(UnmanagedType.R4)]
            [FieldOffset(0xB0)]
            public float heading;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x174)]
            public int ModelID;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x188)]
            public ENTITYSTATUS PlayerStatus;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x189)]
            public bool IsGM;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x18A)]
            public byte icon;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x195)]
            public STATUS IsEngaged;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0xD58)]
            public int TargetId;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x1690)]
            public int cHP;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x1694)]
            public int mHP;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x1698)]
            public int cMP;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x169C)]
            public int mMP;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x16A0)]
            public short cTP;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x16A2)]
            public short cGP;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x16A4)]
            public short mGP;

        };
        public Entity(ENTITYINFO _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }
    }
}