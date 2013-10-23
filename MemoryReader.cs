using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ffxivlib
{
    public sealed class MemoryReader
    {
        #region ProcessAccessFlags
        private enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }
        #endregion
        #region Fields
        static MemoryReader instance = null;
        private IntPtr _processHandle;
        private Process ffxiv_process;
        #endregion
        #region [C|D]tor/setters
        MemoryReader(Process ffxiv_process)
        {
            this.ffxiv_process = ffxiv_process;
            OpenProcess((uint)ffxiv_process.Id);
        }
        ~MemoryReader()
        {
            CloseHandle(this._processHandle);
        }
        public static MemoryReader setInstance(Process ffxiv_process)
        {
            if (instance == null)
            {
                instance = new MemoryReader(ffxiv_process);
            }
            return instance;
        }
        public static MemoryReader getInstance()
        {
            if (instance == null)
            {
                throw new Exception("Something terrible happened.");
            }
            return instance;
        }
        #endregion
        #region DllImports
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        private static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);
        #endregion
        #region Write/Read/Close trifecta
        public void CloseHandle()
        {
            int retValue = CloseHandle(_processHandle);
            if (retValue == 0)
                throw new Exception("CloseHandle failed");
        }
        public int WriteAddress(IntPtr memoryAddress, byte[] value)
        {
            int byteswritten;

            try
            {
                WriteProcessMemory(_processHandle, memoryAddress, value, (UInt32)value.Length, out byteswritten);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to WriteAddress: " + ex.Message);
                return 0;
            }
            return byteswritten;
        }
        public byte[] ReadAdress(IntPtr memoryAddress, uint bytesToRead, out int bytesReaded)
        {
            try
            {
                if (bytesToRead > 0)
                {
                    var buffer = new byte[bytesToRead];
                    IntPtr ptrBytesReaded;
                    ReadProcessMemory(_processHandle, memoryAddress, buffer, bytesToRead, out ptrBytesReaded);
                    bytesReaded = ptrBytesReaded.ToInt32();
                    return buffer;
                }
                else
                {
                    bytesReaded = 0;
                    return new byte[] {0, 0, 0, 0};
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in ReadAddress Function: " + ex.Message);
                bytesReaded = 0;
                return new byte[] {0, 0, 0, 0};
            }
        }
        #endregion
        #region Helper methods
        private void OpenProcess(uint FFXIVPID)
        {
            try
            {
                _processHandle = OpenProcess(ProcessAccessFlags.All, false, FFXIVPID);
            }
            catch
            {
            }
        }
        public IntPtr ResolveAddress(IntPtr pointer)
        {
            int outres;
            var structure = ReadAdress(pointer, 4, out outres);
            var target = (IntPtr)BitConverter.ToInt32(structure, 0);
            return target;
        }
        public IntPtr GetArrayStart(List<int> path)
        {
            IntPtr currentPtr = ffxiv_process.MainModule.BaseAddress;
            IntPtr result = IntPtr.Zero;
            foreach (int pointer in path)
            {
                currentPtr += pointer;
            }
            return currentPtr;
        }
        public T CreateStructFromAddress<T>(IntPtr address)
        {
            int outres;
            T structure = default(T);

            var ffxiv_structure = address;
            if (ffxiv_structure != IntPtr.Zero)
            {
                var chunk = ReadAdress(ffxiv_structure, (uint)Marshal.SizeOf(typeof(T)), out outres);
                GCHandle handle = GCHandle.Alloc(chunk, GCHandleType.Pinned);
                structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();
            }
            else
            {
                throw new Exception("Nothing at this address.");
            }
            return structure;
        }
        public T CreateStructFromPointer<T>(IntPtr address)
        {
            int outres;
            T structure = default(T);

            var pointer = ReadAdress(address, 4, out outres);
            var ffxiv_structure = (IntPtr)BitConverter.ToInt32(pointer, 0);
            if (ffxiv_structure != IntPtr.Zero)
            {
                var chunk = ReadAdress(ffxiv_structure, (uint)Marshal.SizeOf(typeof(T)), out outres);
                GCHandle handle = GCHandle.Alloc(chunk, GCHandleType.Pinned);
                structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();
            }
            else
            {
                throw new Exception("Nothing at this address.");
            }
            return structure;
        }
        public IntPtr ReadPointerPath(List<int> path)
        {
            IntPtr currentPtr = ffxiv_process.MainModule.BaseAddress;
            IntPtr result = IntPtr.Zero;
            int readBytes;
            var count = path.Count;
            var i = 0;

            foreach (int pointer in path)
            {
                if (++i < count)
                {
                    currentPtr += pointer;
                    var chunk = this.ReadAdress(currentPtr, 4, out readBytes);
                    currentPtr = (IntPtr)BitConverter.ToInt32(chunk, 0);
                }
                else
                {
                    result = currentPtr + pointer;
                }
            }
            return result;
        }
        #endregion
    }
}