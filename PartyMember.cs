using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class PartyMember : BaseObject<PartyMember.PARTYMEMBERINFO>
    {
        #region Constructor

        public PartyMember(PARTYMEMBERINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Properties

        public int PlayerID { get; set; }

        public string Name { get; set; }

        public JOB Job { get; set; }

        public byte Level { get; set; }

        public int CurrentHP { get; set; }

        public int MaxHP { get; set; }

        public int CurrentMP { get; set; }

        public int MaxMP { get; set; }

        public short CurrentTP { get; set; }

        public short CurrentGP { get; set; }

        public short MaxGP { get; set; }

        public BUFF[] Buffs { get; set; }

        public float X { get; set; }

        public float Z { get; set; }

        public float Y { get; set; }

        #endregion

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct PARTYMEMBERINFO
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0)] public int PlayerID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] [FieldOffset(0x10)] public string Name;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x51)] public JOB Job;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x53)] public byte Level;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x58)] public int CurrentHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x5C)] public int MaxHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x60)] public int CurrentMP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x62)] public int MaxMP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x64)] public short CurrentTP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x66)] public short CurrentGP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x68)] public short MaxGP;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)] [FieldOffset(0x70)] public BUFF[] Buffs;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x1F0)] public float X;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x1F4)] public float Z;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x1F8)] public float Y;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x38C)] public int padding;
        };

        #endregion
    }

    public partial class FFXIVLIB
    {
        #region Public methods

        /// <summary>
        /// Deprecated, use getPartyMemberById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartyMember GetPartyMemberInfo(int id)
        {
            return GetPartyMemberById(id);
        }

        /// <summary>
        ///     This function retrieves a PartyMember by its id in the PartyMember array
        ///     The result might be empty, there is no sanity check at the time
        /// </summary>
        /// <param name="id">Position in the PartyMember Array, use Constants.PARTY_MEMBER_ARRAY_SIZE as your max (exclusive)</param>
        /// <returns>PartyMember object</returns>
        /// <exception cref="System.IndexOutOfRangeException">Out of range</exception>
        public PartyMember GetPartyMemberById(int id)
        {
            if (id >= Constants.PARTY_MEMBER_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = _mr.ResolvePointerPath(Constants.PARTYPTR);
            pointer = IntPtr.Add(pointer, Marshal.SizeOf(typeof (PartyMember.PARTYMEMBERINFO))*id);
            var p = new PartyMember(_mr.CreateStructFromAddress<PartyMember.PARTYMEMBERINFO>(pointer), pointer);
            return p;
        }

        #endregion
    }
}