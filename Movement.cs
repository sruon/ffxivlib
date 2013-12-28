using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Movement : BaseObject<Movement.MOVEMENTINFO>
    {
        public Movement(MOVEMENTINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct MOVEMENTINFO
        {
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0)] public bool IsMoving;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x1)] public bool IsHeading;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x2)] public bool IsWalking;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x4)] public bool IsFollowing;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x18)] public float CurrentSpeed;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x20)] public float ForwardSpeed;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x28)] public float SideSpeed;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x30)] public float BackwardSpeed;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x278)] public float Heading;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x284)] public float X;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x288)] public float Z;
            [MarshalAs(UnmanagedType.R4)] [FieldOffset(0x28C)] public float Y;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x524)] public bool IsHeadingLeft;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x530)] public bool IsHeadingRight;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x534)] public bool IsMovingRight;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x564)] public bool IsMovingLeft;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x56C)] public bool IsMovingBackward;
            [MarshalAs(UnmanagedType.I1)] [FieldOffset(0x57C)] public bool IsMovingForward;
        };

        #endregion
    }

    public partial class FFXIVLIB
    {
        public Movement GetMovement()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.MOVEMENTPTR);
            var MovementInfo = new Movement(_mr.CreateStructFromAddress<Movement.MOVEMENTINFO>(pointer), pointer);
            return MovementInfo;
        }
    }
}