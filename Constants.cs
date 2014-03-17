using System;
using System.Collections.Generic;

public static class Constants
{
	// ReSharper disable InconsistentNaming
	internal const string PROCESS_NAME = "ffxiv";

	#region Widget related pointer paths

	// Lobby widget
	internal static readonly List<int> SERVERPTR = new List<int>
	{
		0xE93560,
		0x468,
		0x17a
	};

	// Action bar widget
	internal static readonly List<int> ACTIONPTR = new List<int>
	{
		0xFB11A4,
		0x2A8,
		0x20,
		0x14
	};

	// The actual hotbar
	internal static readonly List<int> HOTBARPTR = new List<int>
	{
		0xE93560,
		0x24C,
		0x10,
		0x2F4,
		0x48
	};

	internal static readonly int HOTBAROFFSET = 0x1640;

	#endregion

	#region Pointer paths
	// Simple representation of pointer paths, do not add base module, last member will be expected to be the offset so may have to add extra 0 at the end if you dont need it
	internal static readonly List<int> PCPTR = new List<int>
	{
		0x0119B514,
		0x0
	};

	internal static readonly List<int> GATHERINGPTR = new List<int>
	{
		0x1194FC8,
		0x0
	};

	internal static readonly List<int> PARTYSIZEPTR = new List<int>
	{
		0xE939E0,
		0x50,
		0x768,
		0x54,
		0x4C,
		0x13C
	};

	internal static readonly int PARTYSIZEADDR = 0x1A7F59C;

	internal static readonly List<int> PARTYPTR = new List<int>
	{
		0xE28C98,
		0x2A8,
		0x1D8,
		0x290
	};

	internal static readonly byte[] PARTYPTRSIG = new byte[] { 0xDB, 0x0F, 0xC9, 0x3F, 0xDB, 0x0F, 0x49, 0x41, 0x6F, 0x12, 0x83, 0x3A, 0xFF, 0xFF, 0xFF, 0xFF, 0xDB, 0x0F, 0xC9, 0x3F, 0xDB, 0x0F, 0x49, 0x41, 0x6F, 0x12, 0x83, 0x3A, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

	internal static readonly List<int> PARTYINBATTLEPTR = new List<int> 
	{
		0xFCE8C0,
		0xC4
	};

	internal static readonly List<int> TARGETPTR = new List<int>
	{
		0xFC4B34,
		0x18
	};

	internal static readonly List<int> PLAYERPTR = new List<int>
	{
		0xED7C0,
		0x0
	};

	internal static readonly int PLAYERADDR = 0xFBB9E0;

	internal static readonly int PLAYERFOLLOWINGADDR = 0x1DEF5D8;

	internal static readonly List<int> CHATPTR = new List<int>
	{
		0xE8D6A8,
		0x18,
		0x204
	};

	internal static readonly List<int> INVENTORYPTR = new List<int>
	{
		0x1195C48,
		0
	};

	internal static readonly List<int> ZONEPTR = new List<int>
	{
		0x8AF5C,
		0x0
	};

	internal static readonly List<int> SUBZONEPTR = new List<int>
	{
		0x8AF5C,
		0x4
	};

	internal static readonly List<int> MOVEMENTPTR = new List<int>
	{
		0x790954,
		0x0
	};
	#endregion

	#region Array size

	internal const uint CHATLOG_ARRAY_SIZE = 1000;
	internal const uint ENTITY_ARRAY_SIZE = 100;
	internal const uint PARTY_MEMBER_ARRAY_SIZE = 8;
	internal const uint GATHERING_ARRAY_SIZE = 40;

	#endregion

	#region Chat related

	internal const int TIMESTAMP_SIZE = 8;
	internal const int CHATCODE_SIZE = 4;

	#endregion

	#region ResourceParser related

	/// <summary>
	/// Some parameters can be overriden BEFORE instanciating FFXIVLIB.
	/// </summary>
	public class ResourceParser
	{
		public static string RESOURCES_FOLDER = "Resources";
		/// <summary>
		/// Valid values : ja, fr, en, de
		/// </summary>
		public static string RESOURCES_LANGUAGE = "en";
		internal const string BUFF_FILE = "Buffs.xml";
		internal const string ITEM_FILE = "Item.xml";
		internal const string TITLE_FILE = "Title.xml";
		internal const string JOB_FILE = "ClassJob.xml";
		internal const string GRAND_COMPANY_FILE = "GrandCompany.xml";
		internal const string GRAND_COMPANY_RANK_FILE = "GCRankUldahMaleText.xml";
		internal const string AUTOTRANSLATE_FILE = "Autotranslate.xml";
		internal const string ZONE_FILE = "PlaceName.xml";
		internal const string QUEST_FILE = "Quest.xml";
		internal const string ACTION_FILE = "Action.xml";
	}

	#endregion
	// ReSharper restore InconsistentNaming
}