using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using ObjectDumper;

namespace ffxivlib
{

    public class FFXIVLIB
    {
        #region Fields
        private Int32 ffxiv_pid;
        private Process ffxiv_process;
        private MemoryReader mr = null;
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
            #endregion
            this.ffxiv_process = ffxiv_process;
            this.ffxiv_pid = ffxiv_process.Id;
            this.mr = new MemoryReader(ffxiv_process);
            Debug.WriteLine("PID is " + this.ffxiv_pid.ToString());
        }

        // Instanciation with PID
        public FFXIVLIB(int pid)
        {
            Process ffxiv_process = Process.GetProcessById(pid);
            #region Sanity checks
            if (!ffxiv_process.MainWindowTitle.Equals(Constants.WINDOW_TITLE))
            {
                throw new System.InvalidOperationException("We're might not be attaching to FFXIV, is something wrong?");
            }
            if (ffxiv_process.MainModule.ModuleMemorySize < Constants.PROCESS_MMS)
            {
                throw new System.InvalidOperationException("Wrong MMS.");
            }
            #endregion
            this.ffxiv_process = ffxiv_process;
            this.ffxiv_pid = ffxiv_process.Id;
            this.mr = new MemoryReader(ffxiv_process);
        }
        #endregion
        #region Static methods

        public Entity.ENTITYINFO getEntityInfo(int id)
        {
            if (id >= Constants.ENTITY_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = mr.GetArrayStart(Constants.PCPTR);
            Console.WriteLine("Adress of ENTITY array is: " + pointer.ToString("X"));
            return mr.CreateStructFromAddress<Entity.ENTITYINFO>(IntPtr.Add(mr.GetArrayStart(Constants.PCPTR), id * 0x4));
        }
        #endregion
    }
}
