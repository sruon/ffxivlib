namespace ffxivlib
{
    

    /// <summary>
    ///     Job ID as used in various structures
    /// </summary>
    public enum JOB : byte
    {
// ReSharper disable InconsistentNaming
        GLD = 0x1,
        PGL = 0x2,
        MRD = 0x3,
        LNC = 0x4,
        ARC = 0x5,
        CNJ = 0x6,
        THM = 0x7,
        CPT = 0x8,
        BSM = 0x9,
        ARM = 0xA,
        GSM = 0xB,
        LTW = 0xC,
        WVR = 0xD,
        ALC = 0xE,
        CUL = 0xF,
        MIN = 0x10,
        BOT = 0x11,
        FSH = 0x12,
        PLD = 0x13,
        MNK = 0x14,
        WAR = 0x15,
        DRG = 0x16,
        BRD = 0x17,
        WHM = 0x18,
        BLM = 0x19,
        ACN = 0x2A,
        SMN = 0x2B,
        SCH = 0x2C,
        Chocobo = 0x2D,
        Pet = 0x2E
        // ReSharper restore InconsistentNaming
    }

    public enum SEX : byte
    {
        Male = 0x0,
        Female = 0x1
    }
    /// <summary>
    ///     Current action of an Entity (PC)
    /// </summary>
    public enum ENTITYSTATUS : byte
    {
        Idle = 0x01,
        Dead = 0x02,
        Sitting = 0x03,
        Mounted = 0x04,
        Crafting = 0x05,
        Gathering = 0x06,
        Melding = 0x07,
        SMachine = 0x08
    }

    /// <summary>
    ///     Status of an Entity (PC/NPC)
    /// </summary>
    public enum STATUS : byte
    {
        Engaged = 0x01,
        Idle = 0x02,
        Crafting = 0x05
    }

    /// <summary>
    ///     Type of the entity
    /// </summary>
    public enum TYPE : byte
    {
        Player = 0x01,
        Mob = 0x02,
        NPC = 0x03,
        Aetheryte = 0x05,
        Gathering = 0x06,
        Minion = 0x09
    }

    /// <summary>
    ///     Icons
    /// </summary>
    public enum ICON : byte
    {
        // I obviously don't know what most of these mean... names are tentative
        None = 0x0,
        Yoshida = 0x1,
        GM = 0x2,
        SGM = 0x3,
        Clover = 0x4,
        Dc = 0x5,
        Smiley = 0x6,
        RedCross = 0x9,
        GreyDc = 0xA,
        Processing = 0xB,
        Busy = 0xC,
        Duty = 0xD,
        ProcessingYellow = 0xE,
        ProcessingGrey = 0xF,
        Cutscene = 0x10,
        Away = 0x12,
        Sitting = 0x13,
        WrenchYellow = 0x14,
        Wrench = 0x15,
        Dice = 0x16,
        ProcessingGreen = 0x17,
        Sword = 0x18,
        AllianceLeader = 0x1A,
        AllianceBlueLeader = 0x1B,
        AllianceBlue = 0x1C,
        PartyLeader = 0x1D,
        PartyMember = 0x1E,
        DutyFinder = 0x18,
        Recruiting = 0x19,
        Sprout = 0x1F,
        Gil = 0x20
    }

    /// <summary>
    ///     Because SE likes to use values that don't make sense.
    /// </summary>
    public enum CURRENTTARGET : byte
    {
        Own = 0x1,
        True = 0x2,
        False = 0x4,
    }

    /// <summary>
    /// This is used for getCurrentEquipment()
    /// </summary>
    public enum EQUIP_POS : byte
    {
        MainHand = 0,
        OffHand = 1,
        Head = 2,
        Body = 3,
        Hands = 4,
        Waist = 5,
        Legs = 6,
        Feet = 7,
        Neck = 8,
        Ears = 9,
        Wrists = 10,
        LeftRing = 11,
        RightRing = 12,
        SoulCrystal = 13
    }
}