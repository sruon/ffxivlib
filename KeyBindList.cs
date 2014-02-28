using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ffxivlib
{
	public class KeyBinds
	{
		public KeyBinds ()
		{
		}

		public bool sendAction(int actionId)
		{

			return false;
		}
	}

	public partial class FFXIVLIB
	{
		public IEnumerable<KeyBind> GetKeyBinds(int id)
		{
			var bindList = new List<KeyBind> ();
			IntPtr pointer = _mr.ResolvePointerPath (Constants.HOTBARPTR);
			pointer += (Constants.HOTBAROFFSET * id);

			for (int i = 0; i < 12; ++i) {
				IntPtr newPointer = pointer + (Marshal.SizeOf (typeof(KeyBind.KeyBindInfo)) * i);
				var temp = new KeyBind(_mr.CreateStructFromAddress<KeyBind.KeyBindInfo> (newPointer), newPointer);
				bindList.Add (temp);
			}

			return bindList;
		}
	}
}

