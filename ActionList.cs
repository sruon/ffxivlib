using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class ActionList : BaseObject<ActionList.ACTIONLIST>
    {
        public ActionList(ACTIONLIST structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #region Properties

        public Action.ACTION[] Actions { get; set; }

        #endregion

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ACTIONLIST
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] [FieldOffset(0x0)] public Action.ACTION[] Actions;
        };

        #endregion
    }

    public partial class FFXIVLIB
    {
        public IEnumerable<Action> GetActions()
        {
            var ActionListRet = new List<Action>();
            IntPtr pointer = _mr.ResolvePointerPath(Constants.ACTIONPTR);
            var i = new ActionList(_mr.CreateStructFromAddress<ActionList.ACTIONLIST>(pointer), pointer);
            int idx = 0;
            foreach (Action.ACTION action in i.Actions)
            {
                ActionListRet.Add(
                    new Action(action,
                        i.Address + (idx*Marshal.SizeOf(typeof (Action.ACTION)))
                        ));
                idx++;
            }
            return ActionListRet;
        }
    }
}