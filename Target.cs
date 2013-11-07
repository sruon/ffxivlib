using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Target : IContainer<Target, Target.TARGET>
    {
        public Target(TARGET _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct TARGET
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0)] public int CurrentTarget;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x18)] public int MouseoverTarget;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x4C)] public int PreviousTarget;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x60)] public int CurrentTargetID;
        }
    }

    public partial class FFXIVLIB
    {
        /// <summary>
        ///     This function retrieves the target array
        /// </summary>
        /// <returns>Target object</returns>
        public Target getTargets()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            return t;
        }

        /// <summary>
        ///     This function retrieves the previous target
        /// </summary>
        /// <returns>Entity object or null</returns>
        public Entity getPreviousTarget()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
                {
                    var e =
                        new Entity(mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.structure.PreviousTarget),
                                   (IntPtr) t.structure.PreviousTarget);
                    return e;
                }
            catch (Exception)
                {
                    return null;
                }
        }

        /// <summary>
        ///     This function retrieves the current Mouseover target
        /// </summary>
        /// <returns>Entity object or null</returns>
        public Entity getMouseoverTarget()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
                {
                    var e =
                        new Entity(mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.structure.MouseoverTarget),
                                   (IntPtr) t.structure.MouseoverTarget);
                    return e;
                }
            catch (Exception)
                {
                    return null;
                }
        }

        /// <summary>
        ///     This function retrieves the current target
        /// </summary>
        /// <returns>Entity object or null</returns>
        public Entity getCurrentTarget()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
                {
                    var e = new Entity(
                        mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.structure.CurrentTarget),
                        (IntPtr) t.structure.CurrentTarget);
                    return e;
                }
            catch (Exception)
                {
                    return null;
                }
        }
    }
}