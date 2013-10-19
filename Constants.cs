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
    public static readonly List<int> OWNPCPTR = new List<int>
    {
        0x010BED9C,
        0x30
    };
    #endregion
}