using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Player : IContainer<Player, Player.PLAYERINFO>
    {
        /*
         * At 0x1 the character name is available, but C# throws a Run time error because it's not aligned on proper boundaries,
         * therefore I'm not making it available for now as that'd require me to unalign pretty much everything else.
         * Refer to ENTITYINFO(0) for name.
         */

        public Player(PLAYERINFO _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct PLAYERINFO
        {
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x64)] public JOB job;

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

            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xE4)] public short baseSTR;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xE8)] public short baseDEX;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xEC)] public short baseVIT;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xF0)] public short baseINT;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xF4)] public short baseMND;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0xF8)] public short basePIE;

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

            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x118)] public int mHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x111C)] public int mMP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x120)] public int mTP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x124)] public int mGP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x128)] public int mCP;

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
        };
    }

    public partial class FFXIVLIB
    {
        /// <summary>
        ///     This function retrieves the current Player info
        /// </summary>
        /// <returns>Player object</returns>
        public Player getPlayerInfo()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.PLAYERPTR);
            var p = new Player(mr.CreateStructFromAddress<Player.PLAYERINFO>(pointer), pointer);
            return p;
        }
    }
}