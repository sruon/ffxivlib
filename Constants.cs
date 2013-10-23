using System;
using System.Collections.Generic;

static class Constants
{
    public const string PROCESS_NAME = "ffxiv";
    public const string WINDOW_TITLE = "FINAL FANTASY XIV: A Realm Reborn";
    // I don't remember why I had to check this, might be useless
    public const Int32 PROCESS_MMS = 10000000;
    // Simple representation of pointer paths, do not add base module, last member will be expected to be the offset so may have to add extra 0 at the end if you dont need it
    #region Pointer paths
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
    public const uint ENTITY_ARRAY_SIZE = 100;
    public const uint PARTY_MEMBER_ARRAY_SIZE = 8;
    #endregion
}