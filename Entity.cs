using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Entity : BaseObject<Entity.ENTITYINFO>
    {
        #region Constructor

        public Entity(ENTITYINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Properties

        public int GatheringType { get; set; }

        public string Name { get; set; }

        public int PCId { get; set; }

        public int NPCId { get; set; }

        public TYPE MobType { get; set; }

        public CURRENTTARGET CurrentTarget { get; set; }

        public byte Distance { get; set; }

        public byte GatheringStatus { get; set; }

        public float X { get; set; }

        public float Z { get; set; }

        public float Y { get; set; }

        public float Heading { get; set; }

        public byte GatheringInvisible { get; set; }

        public int ModelID { get; set; }

        public ENTITYSTATUS PlayerStatus { get; set; }

        public bool IsGM { get; set; }

        public byte Icon { get; set; }

        public STATUS IsEngaged { get; set; }

        public int TargetId { get; set; }

        public byte GrandCompany { get; set; }

        public byte GrandCompanyRank { get; set; }

        public byte Title { get; set; }

        public int CurrentHP { get; set; }

        public int MaxHP { get; set; }

        public int CurrentMP { get; set; }

        public int MaxMP { get; set; }

        public short CurrentTP { get; set; }

        public short CurrentGP { get; set; }

        public short MaxGP { get; set; }

        public short CurrentCP { get; set; }

        public short MaxCP { get; set; }

        public byte Race { get; set; }

        public SEX Sex { get; set; }

        public BUFF[] Buffs { get; set; }

        #endregion

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ENTITYINFO
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x0)] public int GatheringType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] [FieldOffset(0x30)] public string Name;
            // Not exactly PC ID...
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x74)] public int PCId;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x78)] public int NPCId;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x8A)] public TYPE MobType;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x8C)] public CURRENTTARGET CurrentTarget;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x8D)] public byte Distance;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x8E)] public byte GatheringStatus;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0xA0)] public float X;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0xA4)] public float Z;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0xA8)] public float Y;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0xB0)] public float Heading;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x11C)] public byte GatheringInvisible;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x174)] public int ModelID;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x188)] public ENTITYSTATUS PlayerStatus;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x189)] public bool IsGM;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x18A)] public byte Icon;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x195)] public STATUS IsEngaged;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xD58)] public int TargetId;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x168A)] public byte GrandCompany;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x168B)] public byte GrandCompanyRank;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x168E)] public byte Title;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x1690)] public int CurrentHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x1694)] public int MaxHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x1698)] public int CurrentMP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x169C)] public int MaxMP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x16A0)] public short CurrentTP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x16A2)] public short CurrentGP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x16A4)] public short MaxGP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x16A6)] public short CurrentCP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x16A8)] public short MaxCP;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x2DB8)] public byte Race;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x2DB9)] public SEX Sex;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)] [FieldOffset(0x2F48)] public BUFF[] Buffs;
        };

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the distance between current Entity and a given Entity
        /// </summary>
        /// <param name="other">Entity to compare to</param>
        /// <returns>Distance</returns>
        public float GetDistanceTo(Entity other)
        {
            float fDistX = Math.Abs(Structure.X - other.Structure.X);
            float fDistY = Math.Abs(Structure.Y - other.Structure.Y);
            float fDistZ = Math.Abs(Structure.Z - other.Structure.Z);
            return (float) Math.Sqrt((fDistX*fDistX) + (fDistY*fDistY) + (fDistZ*fDistZ));
        }

        #endregion
    }

    public partial class FFXIVLIB
    {
        #region Public methods

        /// <summary>
        ///     This function build an Entity object according to the position in the Entity array
        ///     You may effectively loop by yourself on this function.
        /// </summary>
        /// <param name="id">Position in the Entity Array, use Constants.ENTITY_ARRAY_SIZE as your max (exclusive)</param>
        /// <returns>Entity object or null</returns>
        /// <exception cref="System.IndexOutOfRangeException">Out of range</exception>
        public Entity GetEntityById(int id)
        {
            if (id >= Constants.ENTITY_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = IntPtr.Add(_mr.GetArrayStart(Constants.PCPTR), id*0x4);
            try
                {
                    var e = new Entity(_mr.CreateStructFromPointer<Entity.ENTITYINFO>(pointer),
                        _mr.ResolvePointer(pointer));
                    return e;
                }
            catch (Exception)
                {
                    return null;
                }
        }

        /// <summary>
        /// Deprecated, use getEntityById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entity GetEntityInfo(int id)
        {
            return GetEntityById(id);
        }

        /// <summary>
        ///     This function attempts to retrieve a list of Entity by its name in the Entity array
        ///     This is potentially a costly call as we build a complete list to look for the Entity.
        /// This doesn't include Gathering nodes at the moment. To be fixed.
        /// </summary>
        /// <param name="name">Name of the Entity to be retrieved</param>
        /// <returns>Enumerable list of Entity object or</returns>
        public IEnumerable<Entity> GetEntityByName(string name)
        {
            IntPtr pointer = _mr.GetArrayStart(Constants.PCPTR);
            var entityList = new List<Entity>();
            for (int i = 0; i < Constants.ENTITY_ARRAY_SIZE; i++)
                {
                    IntPtr address = pointer + (i*0x4);
                    try
                        {
                            entityList.Add(new Entity(_mr.CreateStructFromPointer<Entity.ENTITYINFO>(address), address));
                        }
                    catch (Exception)
                        {
                            // No Entity at this position
                        }
                }
            var results = entityList.Where(obj => obj.Structure.Name == name);
            return results;
        }

        /// <summary>
        /// Retrieves a list of Entity corresponding to the given TYPE
        /// Needs to be refactored.
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <returns>Enumerable list of Entity objects</returns>
        public IEnumerable<Entity> GetEntityByType(TYPE type)
        {
            var pointerPath = Constants.PCPTR;
            uint arraySize = Constants.ENTITY_ARRAY_SIZE;
            if (type == TYPE.Gathering)
                {
                    pointerPath = Constants.GATHERINGPTR;
                    arraySize = Constants.GATHERING_ARRAY_SIZE;
                }
            IntPtr pointer = _mr.GetArrayStart(pointerPath);
            var entityList = new List<Entity>();
            for (int i = 0; i < arraySize; i++)
                {
                    IntPtr address = pointer + (i*0x4);
                    try
                        {
                            entityList.Add(new Entity(_mr.CreateStructFromPointer<Entity.ENTITYINFO>(address), address));
                        }
                    catch (Exception)
                        {
                            // No Entity at this position
                        }
                }
            var results = entityList.Where(e => e.Structure.MobType == type);
            return results;
        }

        #endregion
    }
}