using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class IContainer<T, U>
    {
        protected IntPtr address;
        public U structure;

        public void modify<X>(string variable, X value)
        {
            IntPtr tobemodified = IntPtr.Add(address, (int)Marshal.OffsetOf(typeof(U), variable));
            Console.WriteLine("I need to modify {0}", tobemodified.ToString("X"));
                try
                {
                    var byte_value = typeof(BitConverter).GetMethod("GetBytes", new Type[] { value.GetType() })
                    .Invoke(null, new object[] { value });
                    MemoryReader.getInstance().WriteAddress(tobemodified, byte_value as byte[]);
                }
                catch (Exception ex)
                {
                    var byte_value = typeof(BitConverter).GetMethod("GetBytes", new Type[] { typeof(char) })
                    .Invoke(null, new object[] { value });
                    MemoryReader.getInstance().WriteAddress(tobemodified, byte_value as byte[]);
                }

            }
    }
}
