using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    /// <summary>
    ///     Abstract base class for various objects
    /// </summary>
    /// <typeparam name="TU">Structure type</typeparam>
    public abstract class BaseObject<TU>
    {
        #region Properties

        /// <summary>
        /// Address of object in FFXIV memory space.
        /// </summary>
        public IntPtr Address { get; private set; }

        /// <summary>
        /// Structure related to object.
        /// </summary>
        public TU Structure { get; internal set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Base class constructor, sets a few fields.
        /// Cannot be instantiated on its own.
        /// </summary>
        /// <param name="structure">Structure</param>
        /// <param name="address">Address</param>
        internal BaseObject(TU structure, IntPtr address)
        {
            Structure = structure;
            Address = address;
        }

        #endregion

        #region PostConstructor

        /// <summary>
        /// Sets object properties from structure.
        /// </summary>
        internal void Initialize()
        {
            foreach (var field in typeof (TU).GetFields(BindingFlags.Instance |
                                                       BindingFlags.NonPublic |
                                                       BindingFlags.Public))
                {
                    var value = field.GetValue(Structure);
                    var prop = GetType().GetProperty(field.Name);
                    if (prop != null)
                        {
                            prop.SetValue(this, value, null);
                        }
                }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function computes the address inside FFXIV process space to be modified for a given field
        ///     and then modifies it.
        ///     This will be deprecated with proper setters.
        /// </summary>
        /// <typeparam name="TX">Type of the value to modify, stick to base types</typeparam>
        /// <param name="field">Name of the structure field to modify</param>
        /// <param name="value">Value to assign to field</param>
        public virtual void Modify<TX>(string field, TX value)
        {
            IntPtr tobemodified = IntPtr.Add(Address, (int) Marshal.OffsetOf(typeof (TU), field));
            try
                {
                    object byteValue = typeof (BitConverter).GetMethod("GetBytes", new[] {value.GetType()})
                        .Invoke(null, new object[] {value});
                    MemoryReader.GetInstance().WriteAddress(tobemodified, byteValue as byte[]);
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
                    var byteArray = new byte[1];
                    byteArray[0] = Convert.ToByte(value);
                    MemoryReader.GetInstance().WriteAddress(tobemodified, byteArray);
                }
        }

        /// <summary>
        /// This refreshes the instance
        /// It may have unexpected behavior if address changes.
        /// This will be deprecated with proper getters.
        /// </summary>
        public virtual void Refresh()
        {
            Structure = MemoryReader.GetInstance().CreateStructFromAddress<TU>(Address);
        }

        #endregion
    }
}