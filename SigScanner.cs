using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    internal class SigScanner
    {
        private const int SearchBlockSize = 1024*100;
        private readonly bool _closeOnDispose;
        private readonly uint _oldPermissions;
        private readonly IntPtr _processHandle;
        private readonly IntPtr _processStart;
        private IntPtr _upperMemoryBound;

        public SigScanner(int processID, bool allowClose)
        {
            _processHandle = IntPtr.Zero;
            _processHandle = OpenProcess(ProcessAccessFlags.All, false, processID);
            if (IntPtr.Zero == _processHandle)
                throw new Exception(string.Format("Unable to open process {0}.", processID));
            _closeOnDispose = allowClose;
            Process p = Process.GetProcessById(processID);
            _processStart = p.MainModule.BaseAddress;
            _upperMemoryBound = new IntPtr(p.WorkingSet64);

            VirtualProtectEx(_processHandle, _processStart, _upperMemoryBound, ProcessAccessFlags.All,
                out _oldPermissions);
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
            VirtualProtectEx(_processHandle, _processStart, _upperMemoryBound, (ProcessAccessFlags) _oldPermissions,
                             out perms);
            if (_processHandle != IntPtr.Zero && _closeOnDispose)
                CloseHandle(_processHandle);
        }

        public IntPtr SigScan(byte[] signature)
        {
            Debug.WriteLine((string.Format("Starting scan for:\n{0}\n", BitConverter.ToString(signature))));
            IntPtr currentLoc = _processStart;
            int searchBlockOverride = 0;
            int byteMatches = 0;

            while (true)
                {
                    Debug.WriteLine(
                        (string.Format("Current Loc {0} Block Size: {1}\n", currentLoc.ToInt64().ToString("X2"),
                                       (searchBlockOverride != 0) ? searchBlockOverride : SearchBlockSize)));
                    var memoryChunk = new byte[SearchBlockSize];
                    int readbytes;
                    while (true)
                        {
                            ReadProcessMemory(_processHandle, currentLoc, memoryChunk,
                                              (searchBlockOverride != 0) ? searchBlockOverride : SearchBlockSize,
                                              out readbytes);
                            if (readbytes == 0)
                                {
                                    // Read 1000 bytes after the last readprocessmemory failed
                                    // This is a quick hack and will fail if sig is within the 1000 bytes range
                                    currentLoc = new IntPtr(currentLoc.ToInt64() + 0x1000);
                                }
                            else
                                break;
                        }

                    //make sure we read something
                    if (readbytes <= 0)
                        {
                            Debug.WriteLine("Error: readBytes: {0}\n", readbytes);
                            return IntPtr.Zero;
                        }
                    //sanity check to aviod running out of process
                    int blockSize = (readbytes < SearchBlockSize) ? readbytes : SearchBlockSize;
                    //                Debug.WriteLine(string.Format("Bytes read: {0} ",(readbytes < SearchBlockSize) ? readbytes : SearchBlockSize));
                    //walk the memoryChunk
                    for (int byteLoc = 0; byteLoc < blockSize; byteLoc++)
                        {
                            if (memoryChunk[byteLoc] == signature[byteMatches] || signature[byteMatches] == 0x99)
                                {
                                    //Debug.WriteLine("+");
                                    //match or wildcard inc byteMatches
                                    byteMatches++;
                                    //matched the whole sig
                                    if (byteMatches == signature.Length)
                                        return new IntPtr(currentLoc.ToInt64() + byteLoc - (signature.Length - 1));
                                }
                            else
                                byteMatches = 0;
                        }

                    //sanity check to make sure we don't read outside of the process causing an access volation
                    if (currentLoc.ToInt64() >= _upperMemoryBound.ToInt64()) /*Out of process*/
                        {
                            Debug.WriteLine("Error: out of memory\n");
                            return IntPtr.Zero;
                        }
                    //make sure the next read doesn't run out of process
                    if ((currentLoc.ToInt64() + SearchBlockSize) >= _upperMemoryBound.ToInt64())
                        {
                            //make the next block the size of the remaining memory.
                            searchBlockOverride =
                                Convert.ToInt32((currentLoc.ToInt64() + SearchBlockSize) - _upperMemoryBound.ToInt64());
                            Debug.WriteLine("SearchBlockOverride: {0}\n", searchBlockOverride);
                        }
                    //inc the starting point to the end of what we just searched. This will also allow us to match sigs accross searchBlocks
                    currentLoc = blockSize != SearchBlockSize ? new IntPtr(currentLoc.ToInt64() + blockSize) : new IntPtr(currentLoc.ToInt64() + SearchBlockSize);
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
    }

    public partial class FFXIVLIB
    {
        /// <summary>
        ///     Finds address of specified signature
        ///     This hasnt been tested in a long time
        /// </summary>
        /// <param name="signature">Signature to look for</param>
        /// <returns>IntPtr of address found or IntPtr.Zero</returns>
        public IntPtr GetSigScan(byte[] signature)
        {
            return _ss.SigScan(signature);
        }
    }
}