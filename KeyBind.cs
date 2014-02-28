using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
	public class KeyBind : BaseObject<KeyBind.KeyBindInfo>
	{
		public KeyBind (KeyBindInfo structure, IntPtr address)
			: base (structure, address)
		{
			Initialize ();
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		public struct KeyBindInfo
		{
			[MarshalAs(UnmanagedType.AnsiBStr)] [FieldOffset(0x0)] public String keyBindString;
		};
	}
}

