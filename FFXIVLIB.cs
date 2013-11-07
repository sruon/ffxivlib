using System;
using System.Diagnostics;

namespace ffxivlib
{
    public partial class FFXIVLIB
    {
        #region Fields

        private readonly Int32 ffxiv_pid;
        private readonly MemoryReader mr;
        internal readonly SendKeyInput ski;
        private readonly SigScanner ss;
        private readonly Process ffxiv_process;
        private MovementHelper mh;

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a FFXIVLIB instance.
        /// PID is optionnal but required if multiple FFXIV process are running.
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
                    else if (p.Length > 1)
                        throw new NotImplementedException("Call the constructor with PID if multiple process.");
                    ffxiv_process = p[0];
                }

            #region Sanity checks

            if (!ffxiv_process.MainWindowTitle.Equals(Constants.WINDOW_TITLE))
                throw new InvalidOperationException("We might not be attaching to FFXIV, is something wrong?");
            if (ffxiv_process.MainModule.ModuleMemorySize < Constants.PROCESS_MMS)
                throw new InvalidOperationException("Wrong MMS.");

            #endregion

            ffxiv_pid = ffxiv_process.Id;
            mr = MemoryReader.setInstance(ffxiv_process);
            ss = new SigScanner(ffxiv_pid, true);
            ski = SendKeyInput.setInstance(ffxiv_process.MainWindowHandle);
        }

        #endregion
    }
}