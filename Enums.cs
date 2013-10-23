namespace ffxivlib
{
    public enum JOB : byte
    {
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
    }
    public enum BUFF : byte
    {
    }
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
    public enum STATUS : byte
    {
        Idle = 0x02,
        Engaged = 0x06
    }
    public enum TYPE : byte
    {
        Player = 0x01,
        Mob = 0x02,
        NPC = 0x03
    }
}