using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public class FFXIVLIB
    {
        #region Fields

        private readonly Int32 ffxiv_pid;
        private readonly MemoryReader mr;
        private readonly SendKeyInput ski;
        private readonly SigScanner ss;
        private Process ffxiv_process;

        #endregion

        #region Constructors

        /// <summary>
        ///     Instantiates FFXIVLIB.
        /// </summary>
        /// <remarks>There can only be one FFXIV process running for this to work.</remarks>
        public FFXIVLIB()
        {
            Process[] p = Process.GetProcessesByName(Constants.PROCESS_NAME);
            if (p.Length <= 0)
            {
                throw new InvalidOperationException("No FFXIV process.");
            }
            else if (p.Length > 1)
            {
                throw new NotImplementedException("Call the constructor with PID if multiple process.");
            }
            Process ffxiv_process = p[0];

            #region Sanity checks

            if (!ffxiv_process.MainWindowTitle.Equals(Constants.WINDOW_TITLE))
            {
                throw new InvalidOperationException("We might not be attaching to FFXIV, is something wrong?");
            }
            if (ffxiv_process.MainModule.ModuleMemorySize < Constants.PROCESS_MMS)
            {
                throw new InvalidOperationException("Wrong MMS.");
            }

            #endregion

            this.ffxiv_process = ffxiv_process;
            ffxiv_pid = ffxiv_process.Id;
            mr = MemoryReader.setInstance(ffxiv_process);
            ss = new SigScanner(ffxiv_pid, true);
            ski = new SendKeyInput(ffxiv_process.MainWindowHandle);
        }

        /// <summary>
        ///     Instantiates FFXIVLIB with a given PID.
        /// </summary>
        /// <param name="pid">FFXIV PID</param>
        public FFXIVLIB(int pid)
        {
            Process ffxiv_process = Process.GetProcessById(pid);

            #region Sanity checks

            if (!ffxiv_process.MainWindowTitle.Equals(Constants.WINDOW_TITLE))
            {
                throw new InvalidOperationException("We might not be attaching to FFXIV, is something wrong?");
            }
            if (ffxiv_process.MainModule.ModuleMemorySize < Constants.PROCESS_MMS)
            {
                throw new InvalidOperationException("Wrong MMS.");
            }

            #endregion

            this.ffxiv_process = ffxiv_process;
            ffxiv_pid = ffxiv_process.Id;
            mr = MemoryReader.setInstance(ffxiv_process);
            ss = new SigScanner(ffxiv_pid, true);
        }

        #endregion

        /// <summary>
        ///     This function sends a keystroke to the Final Fantasy XIV window
        /// </summary>
        /// <param name="key">Key to press (see Virtual Key Codes for information)</param>
        /// <param name="delay">(Optional) Delay between keypress down and keypress up</param>
        public void SendKey(IntPtr key, int delay = 100)
        {
            ski.SendKeyPress((SendKeyInput.VKKeys) key, delay);
        }

        /// <summary>
        ///     This function build an Entity object according to the position in the Entity array
        ///     You may effectively loop by yourself on this function.
        /// </summary>
        /// <param name="id">Position in the Entity Array, use Constants.ENTITY_ARRAY_SIZE as your max (exclusive)</param>
        /// <returns>Entity object or null</returns>
        /// <exception cref="System.IndexOutOfRangeException">Out of range</exception>
        public Entity getEntityInfo(int id)
        {
            if (id >= Constants.ENTITY_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = IntPtr.Add(mr.GetArrayStart(Constants.PCPTR), id*0x4);
            try
            {
                var e = new Entity(mr.CreateStructFromPointer<Entity.ENTITYINFO>(pointer), mr.ResolvePointer(pointer));
                return e;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     This function attempts to retrieve an Entity by its name in the Entity array
        ///     This is potentially a costly call as we build a complete list to look for the Entity.
        /// </summary>
        /// <param name="name">Name of the Entity to be retrieved</param>
        /// <returns>Entity object or null</returns>
        public Entity getEntityByName(string name)
        {
            IntPtr pointer = mr.GetArrayStart(Constants.PCPTR);
            var entity_list = new List<Entity>();
            for (int i = 0; i < Constants.ENTITY_ARRAY_SIZE; i++)
            {
                IntPtr address = pointer + (i*0x4);
                try
                {
                    entity_list.Add(new Entity(mr.CreateStructFromPointer<Entity.ENTITYINFO>(address), address));
                }
                catch (Exception)
                {
                    // No Entity at this position
                }
            }
            Entity result = entity_list.SingleOrDefault(obj => obj.structure.name == name);
            return result;
        }

        /// <summary>
        ///     This function retrieves a PartyMember by its id in the PartyMember array
        ///     The result might be empty, there is no sanity check at the time
        /// </summary>
        /// <param name="id">Position in the PartyMember Array, use Constants.PARTY_MEMBER_ARRAY_SIZE as your max (exclusive)</param>
        /// <returns>PartyMember object</returns>
        /// <exception cref="System.IndexOutOfRangeException">Out of range</exception>
        public PartyMember getPartyMemberInfo(int id)
        {
            if (id >= Constants.PARTY_MEMBER_ARRAY_SIZE)
                throw new IndexOutOfRangeException();
            IntPtr pointer = mr.ResolvePointerPath(Constants.PARTYPTR);
            pointer = IntPtr.Add(pointer, Marshal.SizeOf(typeof (PartyMember.PARTYMEMBERINFO))*id);
            var p = new PartyMember(mr.CreateStructFromAddress<PartyMember.PARTYMEMBERINFO>(pointer), pointer);
            return p;
        }

        /// <summary>
        ///     This function retrieves the current target
        /// </summary>
        /// <returns>Entity object or null</returns>
        public Entity getCurrentTarget()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
            {
                var e = new Entity(mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.structure.CurrentTarget),
                                   (IntPtr) t.structure.CurrentTarget);
                return e;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     This function instantiates a Chatlog object
        /// </summary>
        /// <returns>Chatlog instance</returns>
        public Chatlog getChatlog()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.CHATPTR);
            var c = new Chatlog(mr.CreateStructFromAddress<Chatlog.CHATLOGINFO>(pointer), pointer);
            return c;
        }

        /// <summary>
        ///     This function retrieves the target array
        /// </summary>
        /// <returns>Target object</returns>
        public Target getTargets()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            return t;
        }

        /// <summary>
        ///     This function retrieves the previous target
        /// </summary>
        /// <returns>Entity object or null</returns>
        public Entity getPreviousTarget()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
            {
                var e = new Entity(mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.structure.PreviousTarget),
                                   (IntPtr) t.structure.PreviousTarget);
                return e;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     This function retrieves the current Mouseover target
        /// </summary>
        /// <returns>Entity object or null</returns>
        public Entity getMouseoverTarget()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.TARGETPTR);
            var t = new Target(mr.CreateStructFromAddress<Target.TARGET>(pointer), pointer);
            try
            {
                var e = new Entity(mr.CreateStructFromAddress<Entity.ENTITYINFO>((IntPtr) t.structure.MouseoverTarget),
                                   (IntPtr) t.structure.MouseoverTarget);
                return e;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     This function retrieves the current Player info
        /// </summary>
        /// <returns>Player object</returns>
        public Player getPlayerInfo()
        {
            IntPtr pointer = mr.ResolvePointerPath(Constants.PLAYERPTR);
            var p = new Player(mr.CreateStructFromAddress<Player.PLAYERINFO>(pointer), pointer);
            return p;
        }

        /// <summary>
        ///     Finds address of specified signature
        ///     This hasnt been tested in a long time
        /// </summary>
        /// <param name="signature">Signature to look for</param>
        /// <returns>IntPtr of address found or IntPtr.Zero</returns>
        public IntPtr getSigScan(byte[] signature)
        {
            return ss.SigScan(signature);
        }
    }
}