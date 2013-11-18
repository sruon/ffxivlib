using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ffxivlib
{
    public partial class Chatlog : BaseObject<Chatlog.CHATLOGINFO>
    {
        #region Constructor

        public Chatlog(CHATLOGINFO structure, IntPtr address)
            : base(structure, address)
        {
            Initialize();
        }

        #endregion

        #region Fields

        private readonly List<Entry> _buffer = new List<Entry>();
        private readonly MemoryReader _mr = MemoryReader.GetInstance();
        private readonly List<int> _offsetsList = new List<int>();
        private int _currentOffset;
        private int _previousArrayIndex;
        private int _previousOffset;

        #endregion

        #region Private Methods

        /// <summary>
        ///     Updates our offsets list
        /// </summary>
        private void UpdateOffsetArray()
        {
            _offsetsList.Clear();
            MemoryReader mr = MemoryReader.GetInstance();
            for (int i = 0; i < Constants.CHATLOG_ARRAY_SIZE; i++)
                _offsetsList.Add((int) mr.ResolvePointer((IntPtr) (Structure.ArrayStart + (i*0x4))));
        }

        /// <summary>
        ///     Reads a single line and create an Entry out of it
        /// </summary>
        /// <param name="start">Start offset of the line</param>
        /// <param name="end">End offset of the line</param>
        /// <returns>Entry instance</returns>
        private Entry ReadEntry(int start, int end)
        {
            int bytesread;

            var cle =
                new Entry(_mr.ReadAdress((IntPtr) (Structure.LogStart + start), (uint) (end - start), out bytesread));
            return cle;
        }

        /// <summary>
        ///     This is a wrapper around ReadEntry
        ///     in order to create a List of Entry out
        ///     of all the lines we haven't processed yet.
        /// </summary>
        /// <param name="start">Array index to start at</param>
        /// <param name="end">Array index to stop at</param>
        /// <returns>List of Entry instances</returns>
        private IEnumerable<Entry> ReadEntries(int start, int end)
        {
            var ret = new List<Entry>();
            for (int i = start; i < end; i++)
                {
                    _currentOffset = _offsetsList[i];
                    ret.Add(ReadEntry(_previousOffset, _currentOffset));
                    _previousOffset = _currentOffset;
                }
            return ret;
        }

        /// <summary>
        ///     This updates our internal buffer.
        /// </summary>
        private void UpdateBuffer()
        {
            UpdateOffsetArray();
            Structure = _mr.CreateStructFromAddress<CHATLOGINFO>(Address);
            int currentArrayIndex = (Structure.ArrayCurrent - Structure.ArrayStart) / 4;

            // I forgot why we did this
            if (currentArrayIndex < _previousArrayIndex)
            {
                _buffer.AddRange(ReadEntries(_previousArrayIndex, (int)Constants.CHATLOG_ARRAY_SIZE));
                _previousOffset = 0;
                _previousArrayIndex = 0;
            }
            if (_previousArrayIndex < currentArrayIndex)
                _buffer.AddRange(ReadEntries(_previousArrayIndex, currentArrayIndex));
            _previousArrayIndex = currentArrayIndex;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Is there a new line?
        /// </summary>
        /// <returns></returns>
        public bool IsNewLine()
        {
            UpdateBuffer();
            return (_buffer.Count > 0);
        }

        /// <summary>
        ///     This returns a copy of our buffer, and clear it.
        /// </summary>
        /// <returns>List of Entry instances</returns>
        public List<Entry> GetChatLogLines()
        {
            UpdateBuffer();
            List<Entry> newList = _buffer.ToList();
            _buffer.Clear();
            return newList;
        }

        #endregion

        #region Unmanaged structure

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct CHATLOGINFO
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0)] public int Count;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x20)] public int ArrayStart;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x24)] public int ArrayCurrent;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x30)] public int LogStart;
        };

        #endregion
    }

    public partial class FFXIVLIB
    {
        #region Public methods

        /// <summary>
        ///     This function instantiates a Chatlog object
        /// </summary>
        /// <returns>Chatlog instance</returns>
        public Chatlog GetChatlog()
        {
            IntPtr pointer = _mr.ResolvePointerPath(Constants.CHATPTR);
            var c = new Chatlog(_mr.CreateStructFromAddress<Chatlog.CHATLOGINFO>(pointer), pointer);
            return c;
        }

        #endregion
    }
}