using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ffxivlib
{
    public class FFXIVLIB
    {
        #region Fields
        private Int32 ffxiv_pid;
        #endregion
        #region Constructors
        // Instanciation without PID
        public FFXIVLIB()
        {
            Process[] p = Process.GetProcessesByName(Constants.PROCESS_NAME);
            if (p.Length <= 0)
            {
                throw new System.InvalidOperationException("No FFXIV process.");
            }
            else if (p.Length > 1)
            {
                throw new System.NotImplementedException("Sorry I don't handle multiple PID yet.");
            }
            Process ffxiv_process = p[0];
            #region Sanity checks
            if (!ffxiv_process.MainWindowTitle.Equals(Constants.WINDOW_TITLE))
            {
                throw new System.InvalidOperationException("We're might not be attaching to FFXIV, is something wrong?");
            }
            if (ffxiv_process.MainModule.ModuleMemorySize < Constants.PROCESS_MMS)
            {
                throw new System.InvalidOperationException("Wrong MMS.");
            }
            this.ffxiv_pid = ffxiv_process.Id;
            Console.WriteLine("PID is " + this.ffxiv_pid.ToString());
            #endregion
        }

        // Instanciation with PID
        public FFXIVLIB(int pid)
        {
            throw new System.NotImplementedException("NIE");
        }
        #endregion
        #region Static methods
        static public string Test()
        {
            return "Test";
        }
        #endregion
    }
}
