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
    public partial class FFXIVLIB
    {
        /// <summary>
        ///     This function retrieves a PartyMember by its id in the PartyMember array
        ///     The result might be empty, there is no sanity check at the time
        /// </summary>
        /// <param name="id">Position in the PartyMember Array, use Constants.PARTY_MEMBER_ARRAY_SIZE as your max (exclusive)</param>
        /// <returns>PartyMember object</returns>
        /// <exception cref="System.IndexOutOfRangeException">Out of range</exception>
        public PartyMember getPartyMemberInfo(int id)
        {
            if (id >= Constants.PARTY_MEMBER_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = mr.ResolvePointerPath(Constants.PARTYPTR);
            pointer = IntPtr.Add(pointer, Marshal.SizeOf(typeof(PartyMember.PARTYMEMBERINFO)) * id);
            var p = new PartyMember(mr.CreateStructFromAddress<PartyMember.PARTYMEMBERINFO>(pointer), pointer);
            return p;
        }
    }
}