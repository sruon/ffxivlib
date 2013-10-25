using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class Target : IContainer<Target, Target.TARGET>
    {
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct TARGET
        {
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0)]
            public int CurrentTarget;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x18)]
            public int MouseoverTarget;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x4C)]
            public int PreviousTarget;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x60)]
            public int CurrentTargetID;
        }
        public Target(TARGET _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }
    }
}
