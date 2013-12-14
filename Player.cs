using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ffxivlib
{
    public class Player : BaseObject<Player.PLAYERINFO>
    {
        /*
         * At 0x1 the character name is available, but C# throws a Run time error because it's not aligned on proper boundaries,
         * therefore I'm not making it available for now as that'd require me to unalign everything else.
         * Refer to ENTITYINFO(0) for name.
         */

        #region Constructor

        public Player(PLAYERINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Properties

        public int Zone
        {
            get
            {
                return GetZone();
            } 
        }

        public int Subzone
        {
            get
            {
                return GetSubzone();
            }
        }

        public JOB Job { get; set; }

        #region Job Levels

        public byte PGL { get; set; }

        public byte GLD { get; set; }

        public byte MRD { get; set; }

        public byte ARC { get; set; }

        public byte LNC { get; set; }

        public byte THM { get; set; }

        public byte CNJ { get; set; }

        public byte CPT { get; set; }

        public byte BSM { get; set; }

        public byte ARM { get; set; }

        public byte GSM { get; set; }

        public byte LTW { get; set; }

        public byte WVR { get; set; }

        public byte ALC { get; set; }

        public byte CUL { get; set; }

        public byte MIN { get; set; }

        public byte BOT { get; set; }

        public byte FSH { get; set; }

        public byte ACN { get; set; }

        #endregion

        #region Job Exp In Level

        public int PGL_EIL { get; set; }

        public int GLD_EIL { get; set; }

        public int MRD_EIL { get; set; }

        public int ARC_EIL { get; set; }

        public int LNC_EIL { get; set; }

        public int THM_EIL { get; set; }

        public int CNJ_EIL { get; set; }

        public int ACN_EIL { get; set; }

        public int BSM_EIL { get; set; }

        public int CPT_EIL { get; set; }

        public int GSM_EIL { get; set; }

        public int ARM_EIL { get; set; }

        public int WVR_EIL { get; set; }

        public int LTW_EIL { get; set; }

        public int CUL_EIL { get; set; }

        public int MIN_EIL { get; set; }

        public int BOT_EIL { get; set; }

        public int FSH_EIL { get; set; }

        #endregion

        #region Base Stats

        public short BaseSTR { get; set; }

        public short BaseDEX { get; set; }

        public short BaseVIT { get; set; }

        public short BaseINT { get; set; }

        public short BaseMND { get; set; }

        public short BasePIE { get; set; }

        #endregion

        #region Stats (base+gear+bonus)

        public short STR { get; set; }

        public short DEX { get; set; }

        public short VIT { get; set; }

        public short INT { get; set; }

        public short MND { get; set; }

        public short PIE { get; set; }

        #endregion

        #region Basic infos

        public int MaxHP { get; set; }

        public int MaxMP { get; set; }

        public int MaxTP { get; set; }

        public int MaxGP { get; set; }

        public int MaxCP { get; set; }

        #endregion

        #region Defensive stats

        public short Parry { get; set; }

        public short Defense { get; set; }

        public short Evasion { get; set; }

        public short MagicDefense { get; set; }

        public short SlashingRes { get; set; }

        public short PiercingRes { get; set; }

        public short BluntRes { get; set; }

        public short FireRes { get; set; }

        public short IceRes { get; set; }

        public short WindRes { get; set; }

        public short EarthRes { get; set; }

        public short LightningRes { get; set; }

        public short WaterRes { get; set; }

        #endregion

        #region Offensive stats

        public short AttackPow { get; set; }

        public short Accuracy { get; set; }

        public short CritRate { get; set; }

        public short AttackMagPot { get; set; }

        public short HealMagPot { get; set; }

        public short Determination { get; set; }

        public short SkillSpeed { get; set; }

        public short SpellSpeed { get; set; }

        #endregion

        #region DoH/DoL stats

        public short Craftmanship { get; set; }

        public short Control { get; set; }

        public short Gathering { get; set; }

        public short Perception { get; set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the ZoneID
        /// </summary>
        /// <returns>Zone ID</returns>
        private int GetZone()
        {
            IntPtr ptr = MemoryReader.GetInstance().ResolvePointerPath(Constants.ZONEPTR);
            return MemoryReader.GetInstance().ReadInt4(ptr);
        }

        /// <summary>
        /// Retrieves the Subzone ID
        /// </summary>
        /// <returns>Subzone ID or 0</returns>
        private int GetSubzone()
        {
            IntPtr ptr = MemoryReader.GetInstance().ResolvePointerPath(Constants.SUBZONEPTR);
            return MemoryReader.GetInstance().ReadInt4(ptr);
        }

        #endregion

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct PLAYERINFO
        {
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x64)] public JOB Job;

            #region Job Levels

            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x66)] public byte PGL;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x68)] public byte GLD;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x6A)] public byte MRD;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x6C)] public byte ARC;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x6E)] public byte LNC;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x70)] public byte THM;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x72)] public byte CNJ;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x74)] public byte CPT;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x76)] public byte BSM;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x78)] public byte ARM;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x7A)] public byte GSM;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x7C)] public byte LTW;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x7E)] public byte WVR;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x80)] public byte ALC;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x82)] public byte CUL;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x84)] public byte MIN;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x86)] public byte BOT;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x88)] public byte FSH;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x8A)] public byte ACN;

            #endregion

            #region Job Exp In Level

            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x8C)] public int PGL_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x90)] public int GLD_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x94)] public int MRD_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x98)] public int ARC_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x9C)] public int LNC_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xA0)] public int THM_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xA4)] public int CNJ_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xA8)] public int ACN_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xAC)] public int BSM_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xB0)] public int CPT_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xB4)] public int GSM_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xB8)] public int ARM_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xBC)] public int WVR_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xC0)] public int LTW_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xC4)] public int CUL_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xC8)] public int MIN_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xCC)] public int BOT_EIL;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xD0)] public int FSH_EIL;

            #endregion

            #region Base Stats

            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xE4)] public short BaseSTR;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xE8)] public short BaseDEX;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xEC)] public short BaseVIT;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xF0)] public short BaseINT;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xF4)] public short BaseMND;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xF8)] public short BasePIE;

            #endregion

            #region Stats (base+gear+bonus)

            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x100)] public short STR;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x104)] public short DEX;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x108)] public short VIT;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x10C)] public short INT;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x110)] public short MND;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x114)] public short PIE;

            #endregion

            #region Basic infos

            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x118)] public int MaxHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x111C)] public int MaxMP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x120)] public int MaxTP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x124)] public int MaxGP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x128)] public int MaxCP;

            #endregion

            #region Defensive stats

            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x148)] public short Parry;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x150)] public short Defense;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x158)] public short Evasion;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x15C)] public short MagicDefense;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x170)] public short SlashingRes;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x174)] public short PiercingRes;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x178)] public short BluntRes;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x190)] public short FireRes;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x194)] public short IceRes;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x198)] public short WindRes;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x19C)] public short EarthRes;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x1A0)] public short LightningRes;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x1A4)] public short WaterRes;

            #endregion

            #region Offensive stats

            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x14C)] public short AttackPow;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x154)] public short Accuracy;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x168)] public short CritRate;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x180)] public short AttackMagPot;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x184)] public short HealMagPot;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x1AC)] public short Determination;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x1B0)] public short SkillSpeed;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x1B4)] public short SpellSpeed;

            #endregion

            #region DoH/DoL stats

            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x214)] public short Craftmanship;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x218)] public short Control;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x21C)] public short Gathering;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x220)] public short Perception;

            #endregion

            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x529)] public byte Padding;
        };

        #endregion
    }

    public partial class FFXIVLIB
    {
        #region Public methods

        /// <summary>
        /// TL;DR : Returns current server, testing, might not work reliably.
        /// Longer version : This is set by the Lobby widgets, which no longer exist
        /// while in-game, as such the memory space they used can be under some condition
        /// garbage collected/wiped.
        /// </summary>
        /// <returns></returns>
        public string GetServerName()
        {
            IntPtr ptr = _mr.ResolvePointerPath(Constants.SERVERPTR);
            byte[] server = ReadMemory(ptr, 32);
            string ret = Encoding.UTF8.GetString(server);
            return ret.Split('\0')[0];
        }

        /// <summary>
        ///     This function retrieves the current Player info
        /// </summary>
        /// <returns>Player object</returns>
        public Player GetPlayerInfo()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.PLAYERPTR);
            var p = new Player(_mr.CreateStructFromAddress<Player.PLAYERINFO>(pointer), pointer);
            return p;
        }
        #endregion
    }
}