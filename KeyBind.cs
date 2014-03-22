using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ffxivlib
{
	public class KeyBind : BaseObject<KeyBind.KeyBindInfo>
	{
		public KeyBind (KeyBindInfo structure, IntPtr address)
			: base (structure, address)
		{
			Initialize ();
			switch (structure.keyBindBytes [3])
			{
			case 0xA2:
				keyBindModifier = "CTRL";
				break;
			case 0xA7:
				keyBindModifier = "Shift";
				break;
			case 0xAF:
				keyBindModifier = "ALT";
				break;
			case 0x0:
			default:
				keyBindModifier = "";
				break;
			}

			actionString = Encoding.Default.GetString (new ArraySegment<byte> (structure.actionDescription, 2, 20).ToArray ());
			keyBindString = Encoding.Default.GetString (new ArraySegment<byte> (structure.keyBindBytes, (structure.keyBindBytes[2] == 0xC2)?4:2, 2).ToArray()).Trim();
		}

		public int actionId { get; set; }
		public byte[] actionDescription { get; set; }
		public byte[] keyBindBytes { get; set; }
		public string actionString { get; set; }
		public string keyBindModifier { get; set; }
		public string keyBindString { get; set; }

		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		public struct KeyBindInfo
		{
			[MarshalAs(UnmanagedType.I4)] [FieldOffset(0x0)] public int actionId;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=22)] [FieldOffset(0x1C)] public byte[] actionDescription;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=8)] [FieldOffset(0x120)] public byte[] keyBindBytes;
			[MarshalAs(UnmanagedType.I1)] [FieldOffset(0x163)] public byte padding;
		};
	}
}

