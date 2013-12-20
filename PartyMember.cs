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
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x0)] public float X;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x4)] public float Z;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x8)] public float Y;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x18)] public int PlayerID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] [FieldOffset(0x20)] public string Name;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x61)] public JOB Job;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x63)] public byte Level;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x68)] public int CurrentHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x6C)] public int MaxHP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x70)] public short CurrentMP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x72)] public short MaxMP;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)] [FieldOffset(0x80)] public BUFF[] Buffs;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x39F)] public byte Padding;
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