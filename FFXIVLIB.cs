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
        private SigScanner ss = null;
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
                throw new System.NotImplementedException("Call the constructor with PID if multiple process.");
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
            this.mr = MemoryReader.setInstance(ffxiv_process);
            this.ss = new SigScanner(this.ffxiv_pid, true);
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
            this.mr = MemoryReader.setInstance(ffxiv_process);
            this.ss = new SigScanner(this.ffxiv_pid, true);
        }
        #endregion
        #region Exposed funcs
        public Entity getEntityInfo(int id)
        {
            if (id >= Constants.ENTITY_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = IntPtr.Add(mr.GetArrayStart(Constants.PCPTR), id * 0x4);
            Entity e = new Entity(mr.CreateStructFromPointer<Entity.ENTITYINFO>(pointer), mr.ResolveAddress(pointer));
            return e;
        }
        public PartyMember getPartyMemberInfo(int id)
        {
            if (id >= Constants.PARTY_MEMBER_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = mr.ReadPointerPath(Constants.PARTYPTR);
            pointer = IntPtr.Add(pointer, Marshal.SizeOf(typeof(PartyMember.PARTYMEMBERINFO)) * id);
            PartyMember p = new PartyMember(mr.CreateStructFromAddress<PartyMember.PARTYMEMBERINFO>(pointer), pointer);
            return p;
        }
        public Entity getCurrentTarget()
        {
            IntPtr pointer = mr.ReadPointerPath(Constants.TARGETPTR);
            Entity e = new Entity(mr.CreateStructFromPointer<Entity.ENTITYINFO>(pointer), mr.ResolveAddress(pointer));
            return e;
        }

        public Player getPlayerInfo()
        {
            IntPtr pointer = mr.ReadPointerPath(Constants.PLAYERPTR);
            Player p = new Player(mr.CreateStructFromAddress<Player.PLAYERINFO>(pointer), pointer);
            return p;
        }
        /*
         * This is not useful to us right now, I'm providing it as a helper for people to do custom stuff
         * I have not tested it for a very long time.
         */
        public IntPtr getSigScan(byte[] signature)
        {
            return this.ss.SigScan(signature);
        }
        #endregion
    }
}