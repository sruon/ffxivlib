using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ffxivlib
{
    public class MemoryReader
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

        private IntPtr _processHandle;
        private Process ffxiv_process;
        public MemoryReader(Process ffxiv_process)
        {
            this.ffxiv_process = ffxiv_process;
            OpenProcess((uint)ffxiv_process.Id);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
                                                 [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
                                                 UInt32 dwProcessId);


        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(
            IntPtr hObject
            );

        [DllImport("kernel32.dll")]
        private static extern Int32 ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [In, Out] byte[] buffer,
            UInt32 size,
            out IntPtr lpNumberOfBytesRead
            );

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

        public byte[] ReadAdress(IntPtr memoryAddress, uint bytesToRead,
                                 out int bytesReaded)
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
        public void CloseHandle()
        {
            int retValue = CloseHandle(_processHandle);
            if (retValue == 0)
                throw new Exception("CloseHandle failed");
        }
    }
}