using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ffxivlib
{
	public static class KeyBinds
	{
		public static void sendAction(KeyBind bind)
		{
			SendKeyInput instance = SendKeyInput.GetInstance ();

			SendKeyInput.VKKeys key = (SendKeyInput.VKKeys)bind.keyBindString [0];
			if (bind.keyBindModifier == "Shift")
				instance.ToggleKeyState (SendKeyInput.VKKeys.SHIFT, true);
			if (bind.keyBindModifier == "CTRL")
				instance.ToggleKeyState (SendKeyInput.VKKeys.CONTROL, true);
			instance.SendKeyPress (key);
			if (bind.keyBindModifier == "Shift")
				instance.ToggleKeyState (SendKeyInput.VKKeys.SHIFT, false);
			if (bind.keyBindModifier == "CTRL")
				instance.ToggleKeyState (SendKeyInput.VKKeys.CONTROL, false);
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

