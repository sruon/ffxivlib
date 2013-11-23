using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Companion : BaseObject<Companion.COMPANIONINFO>
    {

        #region Constructor

        public Companion(COMPANIONINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion


        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct COMPANIONINFO
        {
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x118)]
            public int MaxHP;
        };

        #endregion
    }

    public partial class FFXIVLIB
    {
        #region Public methods

        public Player GetCompanionInfo()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.PLAYERPTR);
            IntPtr.Add(pointer, Marshal.SizeOf(typeof(Player.PLAYERINFO)));
            var p = new Player(_mr.CreateStructFromAddress<Player.PLAYERINFO>(pointer), pointer);
            return p;
        }
        #endregion
    }
}