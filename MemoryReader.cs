using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    internal sealed class MemoryReader
    {
        #region ProcessAccessFlags

        private enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            // ReSharper disable UnusedMember.Local
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
            // ReSharper restore UnusedMember.Local
        }

        #endregion

        #region Fields

        private static MemoryReader _instance;
        private readonly Process _ffxivProcess;
        private IntPtr _processHandle;

        #endregion

        #region [C|D]tor/setters

        /// <summary>
        ///     Private constructor
        /// </summary>
        /// <param name="ffxivProcess">FFXIV Process</param>
        private MemoryReader(Process ffxivProcess)
        {
            _ffxivProcess = ffxivProcess;
            OpenProcess((uint) ffxivProcess.Id);
        }

        /// <summary>
        ///     Destructor, make sure we close the handle.
        /// </summary>
        ~MemoryReader()
        {
            CloseHandle(_processHandle);
        }

        /// <summary>
        ///     This is the Singleton instance creator
        /// </summary>
        /// <param name="ffxivProcess">FFXIV Process</param>
        /// <returns>The single MR instance</returns>
        public static MemoryReader SetInstance(Process ffxivProcess)
        {
            return _instance ?? (_instance = new MemoryReader(ffxivProcess));
        }

        /// <summary>
        ///     Retrieve the singleton instance.
        ///     This will choke in its own vomit if called before setInstance()
        /// </summary>
        /// <returns>The single MR instance</returns>
        public static MemoryReader GetInstance()
        {
            if (_instance == null)
                throw new Exception("Something terrible happened.");
            return _instance;
        }

        #endregion

        #region DllImports

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
                                                 [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer,
                                                      UInt32 size, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize,
                                                      out int lpNumberOfBytesWritten);

        #endregion

        #region kernel32 wrapper

        /// <summary>
        ///     Simple wrapper function around WriteProcessMemory. Size is automatically determined.
        /// </summary>
        /// <param name="memoryAddress">Adress to write at in FFXIV memory</param>
        /// <param name="value">Array of byte representing what's to be written</param>
        /// <returns>Amount of bytes written or 0.</returns>
        public int WriteAddress(IntPtr memoryAddress, byte[] value)
        {
            int byteswritten;

            try
                {
                    WriteProcessMemory(_processHandle, memoryAddress, value, (UInt32) value.Length, out byteswritten);
                }
            catch (Exception ex)
                {
                    Debug.WriteLine("Failed to WriteAddress: " + ex.Message);
                    return 0;
                }
            return byteswritten;
        }

        /// <summary>
        ///     Simple wrapper function around ReadProcessMemory
        /// </summary>
        /// <param name="memoryAddress">Adress to read in FFXIV memory</param>
        /// <param name="bytesToRead">Amount of bytes to read</param>
        /// <param name="bytesRead">Out value for the amount of bytes effectively read</param>
        /// <returns>Array of byte representing what was read or [0, 0, 0, 0] on failure.</returns>
        public byte[] ReadAdress(IntPtr memoryAddress, uint bytesToRead, out int bytesRead)
        {
            try
                {
                    if (bytesToRead > 0)
                        {
                            var buffer = new byte[bytesToRead];
                            IntPtr ptrBytesReaded;
                            ReadProcessMemory(_processHandle, memoryAddress, buffer, bytesToRead, out ptrBytesReaded);
                            bytesRead = ptrBytesReaded.ToInt32();
                            return buffer;
                        }
                    bytesRead = 0;
                    return new byte[] {0, 0, 0, 0};
                }
            catch (Exception ex)
                {
                    Debug.WriteLine("Error in ReadAddress Function: " + ex.Message);
                    bytesRead = 0;
                    return new byte[] {0, 0, 0, 0};
                }
        }

        #endregion

        #region Helper methods

        private void OpenProcess(uint ffxivpid)
        {
            try
                {
                    _processHandle = OpenProcess(ProcessAccessFlags.All, false, ffxivpid);
                }
            catch {}
        }

        /// <summary>
        ///     Simple pointer reading
        /// </summary>
        /// <param name="pointer">Pointer to read</param>
        /// <returns>Address pointed</returns>
        public IntPtr ResolvePointer(IntPtr pointer)
        {
            int outres;
            byte[] structure = ReadAdress(pointer, 4, out outres);
            var target = (IntPtr) BitConverter.ToInt32(structure, 0);
            return target;
        }

        /// <summary>
        ///     Follows a pointer path (MLP)
        /// </summary>
        /// <param name="path">List of pointers to follow, last element is expected to be an offset to be added to the final result.</param>
        /// <returns>Final address</returns>
        public IntPtr ResolvePointerPath(List<int> path)
        {
            IntPtr currentPtr = _ffxivProcess.MainModule.BaseAddress;
            IntPtr result = IntPtr.Zero;
            int count = path.Count;
            int i = 0;

            foreach (int pointer in path)
                {
                    if (++i < count)
                        {
                            int readBytes;
                            currentPtr += pointer;
                            byte[] chunk = ReadAdress(currentPtr, 4, out readBytes);
                            currentPtr = (IntPtr) BitConverter.ToInt32(chunk, 0);
                        }
                    else
                        result = currentPtr + pointer;
                }
            return result;
        }

        /// <summary>
        ///     This is pretty useless, adds base module to a single level offset.
        ///     This should be removed in favor of something more generic.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IntPtr GetArrayStart(List<int> path)
        {
            return path.Aggregate(_ffxivProcess.MainModule.BaseAddress, (current, pointer) => current + pointer);
        }

        /// <summary>
        ///     Builds a structure 1:1 from FFXIV Memory
        /// </summary>
        /// <typeparam name="T">Type of the structure to create</typeparam>
        /// <param name="address">Address in FFXIV to start reading from</param>
        /// <returns>Created Structure</returns>
        /// <exception cref="Exception">Throws an exception if address is IntPtr.Zero</exception>
        public T CreateStructFromAddress<T>(IntPtr address)
        {
            T structure = default(T);

            IntPtr ffxivStructure = address;
            if (ffxivStructure != IntPtr.Zero)
                {
                    int outres;
                    byte[] chunk = ReadAdress(ffxivStructure, (uint) Marshal.SizeOf(typeof (T)), out outres);
                    GCHandle handle = GCHandle.Alloc(chunk, GCHandleType.Pinned);
                    structure = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (T));
                    handle.Free();
                }
            else
                throw new Exception("Nothing at this address.");
            return structure;
        }

        /// <summary>
        ///     Builds a structure 1:1 from FFXIV Memory, after dereferencing a pointer.
        ///     This should be removed and simply replaced by ResolvePointer in the primary calls
        /// </summary>
        /// <typeparam name="T">Type of the structure to create</typeparam>
        /// <param name="address">Address of pointer in FFXIV to dereference</param>
        /// <returns>Created Structure</returns>
        /// <exception cref="Exception">Throws an exception if pointer result is IntPtr.Zero</exception>
        public T CreateStructFromPointer<T>(IntPtr address)
        {
            int outres;
            T structure = default(T);

            byte[] pointer = ReadAdress(address, 4, out outres);
            var ffxivStructure = (IntPtr) BitConverter.ToInt32(pointer, 0);
            if (ffxivStructure != IntPtr.Zero)
                {
                    byte[] chunk = ReadAdress(ffxivStructure, (uint) Marshal.SizeOf(typeof (T)), out outres);
                    GCHandle handle = GCHandle.Alloc(chunk, GCHandleType.Pinned);
                    structure = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (T));
                    handle.Free();
                }
            else
                throw new Exception("Nothing at this address.");
            return structure;
        }

        #endregion
    }
    public partial class FFXIVLIB
    {
        public byte[] ReadMemory(IntPtr pointer, uint count)
        {
            int outbytes;
            return _mr.ReadAdress(pointer, count, out outbytes);
        }
    }
}