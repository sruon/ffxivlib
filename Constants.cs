using System;
using System.Collections.Generic;

public static class Constants
{
    //ffxiv.exe + 00f8ebe0 + 0x65A -> Server
// ReSharper disable InconsistentNaming
    internal const string PROCESS_NAME = "ffxiv";

    #region Pointer paths
    // Simple representation of pointer paths, do not add base module, last member will be expected to be the offset so may have to add extra 0 at the end if you dont need it
    internal static readonly List<int> PCPTR = new List<int>
        {
            0x010BED9C,
            0x0
        };

    internal static readonly List<int> GATHERINGPTR = new List<int>
        {
            0x010D5C00,
            0x0
        };

    // This is the widget for party
    internal static readonly List<int> PARTYPTR = new List<int>
        {
            0x0014B590,
            0x0
        };

    internal static readonly List<int> TARGETPTR = new List<int>
        {
            0x00C87524,
            0x28
        };

    internal static readonly List<int> PLAYERPTR = new List<int>
        {
            0x5C570,
            0x0
        };

    internal static readonly List<int> CHATPTR = new List<int>
        {
            0xF875A8,
            0x18,
            0x200
        };
    internal static readonly List<int> INVENTORYPTR = new List<int>
        {
            0x10B95B8,
            -0x8
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

    #region Inventory related

    internal const int INVENTORY_PTR_OFFSET = 0x20;
    internal const int SELF_INVENTORY_SIZE = 4;
    internal const int CURRENT_EQUIPMENT_SIZE = 1;
    internal const int SELF_EXTRA_SIZE = 11;
    internal const int RETAINER_INVENTORY_SIZE = 7;
    internal const int RETAINER_EXTRA_SIZE = 3;
    internal const int ARMORY_CHEST_MH_SIZE = 1;
    internal const int ARMORY_CHEST_SIZE = 12;
    internal const int COMPANY_INVENTORY_SIZE = 3;
    internal const int COMPANY_EXTRA_SIZE = 2;

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
        internal const string GRAND_COMPANY_FILE = "GrandCompany.xml";
        internal const string GRAND_COMPANY_RANK_FILE = "GCRankUldahMaleText.xml";
        internal const string AUTOTRANSLATE_FILE = "Autotranslate.xml";
    }

    #endregion
    // ReSharper restore InconsistentNaming
}