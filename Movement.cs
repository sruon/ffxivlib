using System;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Movement : BaseObject<Movement.MOVEMENTINFO>
    {
        #region Constructor

        public Movement(MOVEMENTINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Properties

        public bool IsMoving { get; set; }

        public bool IsHeading { get; set; }

        public bool IsWalking { get; set; }

        public bool IsFollowing { get; set; }

        public float CurrentSpeed { get; set; }

        public float ForwardSpeed { get; set; }

        public float SideSpeed { get; set; }

        public float BackwardSpeed { get; set; }

        public float Heading { get; set; }

        public float X { get; set; }

        public float Z { get; set; }

        public float Y { get; set; }

        public bool IsHeadingLeft { get; set; }

        public bool IsHeadingRight { get; set; }

        public bool IsMovingRight { get; set; }

        public bool IsMovingLeft { get; set; }

        public bool IsMovingBackward { get; set; }

        public bool IsMovingForward { get; set; }

        #endregion

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

        #region Helper methods

        /// <summary>
        /// Start moving to the right.
        /// </summary>
        public void MoveRight()
        {
            this.Modify("IsMovingRight", true);
        }

        /// <summary>
        /// Start moving to the left.
        /// </summary>
        public void MoveLeft()
        {
            this.Modify("IsMovingLeft", true);
        }

        /// <summary>
        /// Start moving forward.
        /// </summary>
        public void MoveForward()
        {
            this.Modify("IsMovingForward", true);
        }

        /// <summary>
        /// Start moving backward.
        /// </summary>
        public void MoveBackward()
        {
            this.Modify("IsMovingBackward", true);
        }

        /// <summary>
        /// Start heading left.
        /// </summary>
        public void HeadLeft()
        {
            this.Modify("IsHeadingLeft", true);
        }

        /// <summary>
        /// Start heading right.
        /// </summary>
        public void HeadRight()
        {
            this.Modify("IsHeadingRight", true);
        }

        /// <summary>
        /// Stop any movement.
        /// </summary>
        public void StopMoving()
        {
            this.Modify("IsMovingRight", false);
            this.Modify("IsMovingLeft", false);
            this.Modify("IsMovingForward", false);
            this.Modify("IsMovingBackward", false);
            this.Modify("IsHeadingLeft", false);
            this.Modify("IsHeadingRight", false);
        }

        #endregion
    }

    public partial class FFXIVLIB
    {
        public Movement GetMovement()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.MOVEMENTPTR);
            var movementInfo = new Movement(_mr.CreateStructFromAddress<Movement.MOVEMENTINFO>(pointer), pointer);
            return movementInfo;
        }
    }
}