﻿using System;
using System.Diagnostics;

namespace ffxivlib
{
    public partial class FFXIVLIB
    {
        #region Fields

        public readonly Int32 Pid;
        private readonly MemoryReader _mr;
        internal readonly SendKeyInput Ski;
        private readonly SigScanner _ss;
        private readonly Process ffxiv_process;
		private IntPtr entityPointer = IntPtr.Zero;

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a FFXIVLIB instance.
        /// PID is optional but required if multiple FFXIV process are running.
        /// </summary>
        /// <param name="pid">FFXIV PID (optionnal)</param>
        public FFXIVLIB(int pid = 0)
        {
            if (pid != 0)
                ffxiv_process = Process.GetProcessById(pid);
            else
                {
                    Process[] p = Process.GetProcessesByName(Constants.PROCESS_NAME);
                    if (p.Length <= 0)
                        throw new InvalidOperationException("No FFXIV process.");
                    if (p.Length > 1)
                        throw new InvalidOperationException("Call the constructor with PID if multiple process.");
                    ffxiv_process = p[0];
                }

            Pid = ffxiv_process.Id;
            _mr = MemoryReader.SetInstance(ffxiv_process);
            _ss = new SigScanner(Pid, true);
			Ski = SendKeyInput.SetInstance(ffxiv_process.MainWindowHandle);
        }

        #endregion
    }
}