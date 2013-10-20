using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    internal class Player
    {
        public string getName(PlayerS p)
        {
            return p.name;
        }
        public float getXPos(PlayerS p)
        {
            return p.X;
        }
        [StructLayout(LayoutKind.Explicit, Pack=1)]
        public struct PlayerS
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            [FieldOffset(0x30)]
            public string name;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x74)] 
            int MobID;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x8A)] 
            TYPE MobType;
            [MarshalAs(UnmanagedType.R4)]
            [FieldOffset(0xA0)] 
            public float X;
            [MarshalAs(UnmanagedType.R4)]
            [FieldOffset(0xA4)] 
            float Z;
            [MarshalAs(UnmanagedType.R4)]
            [FieldOffset(0xA8)] 
            float Y;
            [MarshalAs(UnmanagedType.R4)]
            [FieldOffset(0xB0)] 
            float heading;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x174)] 
            int ModelID;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x188)]
            PSTATUS PlayerStatus;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x189)]
            bool IsGM;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x18A)]
            byte icon;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0x195)]
            STATUS IsEngaged;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0xD58)]
            int TargetId;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x1690)]
            int cHP;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x1694)]
            int mHP;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x1698)]
            int cMP;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x169C)]
            int mMP;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x16A0)]
            short cTP;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x16A2)]
            short cGP;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0x16A4)]
            short mGP;

        };

        enum PSTATUS : byte
        {
            Idle = 0x01,
            Dead = 0x02,
            Sitting = 0x03,
            Mounted = 0x04,
            Crafting = 0x05,
            Gathering = 0x06,
            Melding = 0x07,
            SMachine = 0x08
        }
        enum STATUS : byte
        {
            Idle = 0x02,
            Engaged = 0x06
        }
        enum TYPE : byte
        {
            Player = 0x01,
            Mob = 0x02,
            NPC = 0x03
        }
    }
}
