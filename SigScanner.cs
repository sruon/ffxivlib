/*
 * Credits to h1pp0 for initial implementation
 * 
*/
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    internal class SigScanner
    {
        private static int SearchBlockSize = 1024*100;
        private readonly bool CloseOnDispose;
        private readonly uint OldPermissions;
        private readonly IntPtr ProcessHandle;
        private readonly IntPtr ProcessStart;
        private IntPtr UpperMemoryBound;

        public SigScanner(int ProcessID, bool AllowClose)
        {
            ProcessHandle = IntPtr.Zero;
            ProcessHandle = OpenProcess(ProcessAccessFlags.All, false, ProcessID);
            if (IntPtr.Zero == ProcessHandle)
                throw new Exception(string.Format("Unable to open process {0}.", ProcessID));
            else
            {
                CloseOnDispose = AllowClose;
                Process P = Process.GetProcessById(ProcessID);
                ProcessStart = P.MainModule.BaseAddress;
                UpperMemoryBound = new IntPtr(P.WorkingSet64);

                VirtualProtectEx(ProcessHandle, ProcessStart, UpperMemoryBound, ProcessAccessFlags.All,
                                 out OldPermissions);
            }
        }

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize,
                                                    ProcessAccessFlags flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
                                                 [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
                                                     int dwSize, out int lpNumberOfBytesRead);

        ~SigScanner()
        {
            uint perms;
            VirtualProtectEx(ProcessHandle, ProcessStart, UpperMemoryBound, (ProcessAccessFlags) OldPermissions,
                             out perms);
            if (ProcessHandle != IntPtr.Zero && CloseOnDispose)
                CloseHandle(ProcessHandle);
        }

        public IntPtr SigScan(byte[] Signature)
        {
            Debug.WriteLine((string.Format("Starting scan for:\n{0}\n", BitConverter.ToString(Signature))));
            IntPtr CurrentLoc = ProcessStart;
            byte[] MemoryChunk;
            int readbytes = 0;
            int SearchBlockOverride = 0;
            int ByteMatches = 0;

            while (true)
            {
                Debug.WriteLine((string.Format("Current Loc {0} Block Size: {1}\n", CurrentLoc.ToInt64().ToString("X2"), (SearchBlockOverride != 0) ? SearchBlockOverride:SearchBlockSize)));
                MemoryChunk = new byte[SearchBlockSize];
                while (true)
                {
                    ReadProcessMemory(ProcessHandle, CurrentLoc, MemoryChunk,
                                      (SearchBlockOverride != 0) ? SearchBlockOverride : SearchBlockSize, out readbytes);
                    if (readbytes == 0)
                    {
                        // Read 1000 bytes after the last readprocessmemory failed
                        // This is a quick hack and will fail if sig is within the 1000 bytes range
                        CurrentLoc = new IntPtr(CurrentLoc.ToInt64() + 0x1000);
                    }
                    else
                    {
                        break;
                    }
                }

                //make sure we read something
                if (readbytes <= 0)
                {
                    Debug.WriteLine(string.Format("Error: readBytes: {0}\n", readbytes));
                    return IntPtr.Zero;
                }
                //sanity check to aviod running out of process
                int BlockSize = (readbytes < SearchBlockSize) ? readbytes : SearchBlockSize;
                //                Debug.WriteLine(string.Format("Bytes read: {0} ",(readbytes < SearchBlockSize) ? readbytes : SearchBlockSize));
                //walk the memoryChunk
                for (int ByteLoc = 0; ByteLoc < BlockSize; ByteLoc++)
                {
                    if (MemoryChunk[ByteLoc] == Signature[ByteMatches] || Signature[ByteMatches] == 0x99)
                    {
                        //Debug.WriteLine("+");
                        //match or wildcard inc byteMatches
                        ByteMatches++;
                        //matched the whole sig
                        if (ByteMatches == Signature.Length)
                            return new IntPtr(CurrentLoc.ToInt64() + ByteLoc - (Signature.Length - 1));
                    }
                    else
                    {
                        ByteMatches = 0;
                    }
                }

                //sanity check to make sure we don't read outside of the process causing an access volation
                if (CurrentLoc.ToInt64() >= UpperMemoryBound.ToInt64()) /*Out of process*/
                {
                    Debug.WriteLine("Error: out of memory\n");
                    return IntPtr.Zero;
                }
                //make sure the next read doesn't run out of process
                if ((CurrentLoc.ToInt64() + SearchBlockSize) >= UpperMemoryBound.ToInt64())
                {
                    //make the next block the size of the remaining memory.
                    SearchBlockOverride =
                        Convert.ToInt32((CurrentLoc.ToInt64() + SearchBlockSize) - UpperMemoryBound.ToInt64());
                    Debug.WriteLine(string.Format("SearchBlockOverride: {0}\n", SearchBlockOverride));
                }
                //inc the starting point to the end of what we just searched. This will also allow us to match sigs accross searchBlocks
                if (BlockSize != SearchBlockSize)
                {
                    CurrentLoc = new IntPtr(CurrentLoc.ToInt64() + BlockSize);
                }
                else
                {
                    CurrentLoc = new IntPtr(CurrentLoc.ToInt64() + SearchBlockSize);
                }
            }

            //if we get here, true is no longer true and the world has ended...
/*
            return IntPtr.Zero;
*/
        }

        #region Nested type: ProcessAccessFlags

        [Flags]
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
    }
}