using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ffxivlib
{
    /// <summary>
    /// Basic managed container for structures and other infos
    /// </summary>
    /// <typeparam name="T">Managed object type</typeparam>
    /// <typeparam name="U">Structure type</typeparam>
    public class IContainer<T, U>
    {
        protected IntPtr address;
        public U structure;
        /// <summary>
        /// This function computes the address inside FFXIV process space to be modified for a given field
        /// and then modifies it.
        /// </summary>
        /// <typeparam name="X">Type of the value to modify, stick to base types</typeparam>
        /// <param name="field">Name of the structure field to modify</param>
        /// <param name="value">Value to assign to field</param>
        public void modify<X>(string field, X value)
        {
            IntPtr tobemodified = IntPtr.Add(address, (int)Marshal.OffsetOf(typeof(U), field));
               try
                {
                    var byte_value = typeof(BitConverter).GetMethod("GetBytes", new Type[] { value.GetType() })
                    .Invoke(null, new object[] { value });
                    MemoryReader.getInstance().WriteAddress(tobemodified, byte_value as byte[]);
                }
                catch (AmbiguousMatchException)
                {
                    /*
                     * This fixes 2 issues:
                     * 1. Reflector cannot determine the proper GetBytes() 
                     * call for single byte values (or I'm just bad)
                     * 2. Hack for single byte values, above code create byte[2] 
                     * array which are then written and cause crashes.
                     * I hate catching exceptions for this kind of shit. 
                     * There is probably something more sexy to be done but it works.
                     */
                    byte[] byte_array = new byte[1];
                    byte_array[0] = Convert.ToByte(value);
                    MemoryReader.getInstance().WriteAddress(tobemodified, byte_array);
                }

            }
    }
}
