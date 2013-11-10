using System;
using System.Collections.Generic;

internal static class Constants
{
    public const string PROCESS_NAME = "ffxiv";
    public const string WINDOW_TITLE = "FINAL FANTASY XIV: A Realm Reborn";
    // I don't remember why I had to check this, might be useless
    public const Int32 PROCESS_MMS = 10000000;

    #region Pointer paths
    // Simple representation of pointer paths, do not add base module, last member will be expected to be the offset so may have to add extra 0 at the end if you dont need it
    public static readonly List<int> PCPTR = new List<int>
        {
            0x010BED9C,
            0x0
        };

    // This is the widget for party
    public static readonly List<int> PARTYPTR = new List<int>
        {
            0x0014B600,
            0x0
        };

    public static readonly List<int> TARGETPTR = new List<int>
        {
            0x00C87644,
            0x28
        };

    public static readonly List<int> PLAYERPTR = new List<int>
        {
            0x5C570,
            0x0
        };

    public static readonly List<int> CHATPTR = new List<int>
        {
            0xF875A8,
            0x18,
            0x200
        };
    public static readonly List<int> INVENTORYPTR = new List<int>
        {
            0x10B95B8,
            -0x8
        };

    #endregion

    #region Array size

    public const uint CHATLOG_ARRAY_SIZE = 1000;
    public const uint ENTITY_ARRAY_SIZE = 100;
    public const uint PARTY_MEMBER_ARRAY_SIZE = 8;

    #endregion

    #region Chat related

    public const int TIMESTAMP_SIZE = 8;
    public const int CHATCODE_SIZE = 4;

    #endregion

    #region Inventory related

    public const int INVENTORY_PTR_OFFSET = 0x20;
    public const int SELF_INVENTORY_SIZE = 4;
    public const int CURRENT_EQUIPMENT_SIZE = 1;
    public const int SELF_EXTRA_SIZE = 11;
    public const int RETAINER_INVENTORY_SIZE = 7;
    public const int RETAINER_EXTRA_SIZE = 3;
    public const int ARMORY_CHEST_MH_SIZE = 1;
    public const int ARMORY_CHEST_SIZE = 12;
    public const int COMPANY_INVENTORY_SIZE = 3;
    public const int COMPANY_EXTRA_SIZE = 2;

    #endregion

}