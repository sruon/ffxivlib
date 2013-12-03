using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Target : BaseObject<Target.TARGET>
    {
        #region Constructor

        public Target(TARGET structure, IntPtr address)
            : base(structure, address) {}

        #endregion

        #region Properties

        public int CurrentTarget { get; set; }

        public int MouseoverTarget { get; set; }

        public int FocusTarget { get; set; }

        public int PreviousTarget { get; set; }

        public int CurrentTargetID { get; set; }

        #endregion

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct TARGET
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0)] public int CurrentTarget;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x18)] public int MouseoverTarget;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x40)] public int FocusTarget;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x4C)] public int PreviousTarget;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x60)] public int CurrentTargetID;
        }

        #endregion
    }

    public partial class FFXIVLIB
    {
        #region Public methods

        /// <summary>
        ///     This function retrieves the target array
        /// </summary>
        /// <returns>Target object</returns>
        public Target GetTargets()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(_mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            return t;
        }

        /// <summary>
        ///     This function retrieves the previous target
        /// </summary>
        /// <returns>Entity object or null</returns>
        public Entity GetPreviousTarget()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(_mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
                {
                    var e =
                        new Entity(_mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.Structure.PreviousTarget),
                            (IntPtr) t.Structure.PreviousTarget);
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
        public Entity GetMouseoverTarget()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(_mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
                {
                    var e =
                        new Entity(_mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.Structure.MouseoverTarget),
                            (IntPtr) t.Structure.MouseoverTarget);
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
        public Entity GetCurrentTarget()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(_mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
                {
                    var e = new Entity(
                        _mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.Structure.CurrentTarget),
                        (IntPtr) t.Structure.CurrentTarget);
                    return e;
                }
            catch (Exception)
                {
                    return null;
                }
        }

        /// <summary>
        ///     This function retrieves the focus target
        /// </summary>
        /// <returns>Entity object or null</returns>
        public Entity GetFocusTarget()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(_mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
                {
                    var e = new Entity(
                        _mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.Structure.FocusTarget),
                        (IntPtr) t.Structure.FocusTarget);
                    return e;
                }
            catch (Exception)
                {
                    return null;
                }
        }

        #endregion
    }
}