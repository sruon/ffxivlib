using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Entity : IContainer<Entity, Entity.ENTITYINFO>
    {
        public Entity(ENTITYINFO _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ENTITYINFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] [FieldOffset(0x30)] public string name;
            // Not exactly PC ID...
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x74)] public int PCID;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x78)] public int NPCID;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x8A)] public TYPE MobType;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x8C)] public CURRENTTARGET currentTarget;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x8D)] public byte distance;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0xA0)] public float X;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0xA4)] public float Z;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0xA8)] public float Y;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0xB0)] public float heading;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x174)] public int ModelID;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x188)] public ENTITYSTATUS PlayerStatus;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x189)] public bool IsGM;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x18A)] public byte icon;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x195)] public STATUS IsEngaged;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0xD58)] public int TargetId;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x1690)] public int cHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x1694)] public int mHP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x1698)] public int cMP;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x169C)] public int mMP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x16A0)] public short cTP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x16A2)] public short cGP;
            [MarshalAs(UnmanagedType.I2)] [FieldOffset(0x16A4)] public short mGP;
        };

        public float getDistanceTo(Entity other)
        {
            float fDistX = Math.Abs(structure.X - other.structure.X);
            float fDistY = Math.Abs(structure.Y - other.structure.Y);
            float fDistZ = Math.Abs(structure.Z - other.structure.Z);
            return (float) Math.Sqrt((fDistX*fDistX) + (fDistY*fDistY) + (fDistZ*fDistZ));
        }
    }

    public partial class FFXIVLIB
    {
        /// <summary>
        ///     This function build an Entity object according to the position in the Entity array
        ///     You may effectively loop by yourself on this function.
        /// </summary>
        /// <param name="id">Position in the Entity Array, use Constants.ENTITY_ARRAY_SIZE as your max (exclusive)</param>
        /// <returns>Entity object or null</returns>
        /// <exception cref="System.IndexOutOfRangeException">Out of range</exception>
        public Entity getEntityInfo(int id)
        {
            if (id >= Constants.ENTITY_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = IntPtr.Add(mr.GetArrayStart(Constants.PCPTR), id*0x4);
            try
                {
                    var e = new Entity(mr.CreateStructFromPointer<Entity.ENTITYINFO>(pointer),
                                       mr.ResolvePointer(pointer));
                    return e;
                }
            catch (Exception)
                {
                    return null;
                }
        }

        /// <summary>
        ///     This function attempts to retrieve an Entity by its name in the Entity array
        ///     This is potentially a costly call as we build a complete list to look for the Entity.
        /// </summary>
        /// <param name="name">Name of the Entity to be retrieved</param>
        /// <returns>Entity object or null</returns>
        public Entity getEntityByName(string name)
        {
            IntPtr pointer = mr.GetArrayStart(Constants.PCPTR);
            var entity_list = new List<Entity>();
            for (int i = 0; i < Constants.ENTITY_ARRAY_SIZE; i++)
                {
                    IntPtr address = pointer + (i*0x4);
                    try
                        {
                            entity_list.Add(new Entity(mr.CreateStructFromPointer<Entity.ENTITYINFO>(address), address));
                        }
                    catch (Exception)
                        {
                            // No Entity at this position
                        }
                }
            Entity result = entity_list.SingleOrDefault(obj => obj.structure.name == name);
            return result;
        }
    }
}