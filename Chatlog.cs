using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ffxivlib
{

    public class Chatlog : IContainer<Chatlog, Chatlog.CHATLOGINFO>
    {

        private int arrayIndex;
        private int currentOffset;
        private int previousOffset;
        private int previousArrayIndex;
        List<int> offsets_list = new List<int>();
        private MemoryReader mr = MemoryReader.getInstance();

        [StructLayout(LayoutKind.Explicit, Pack=1)]
        public struct CHATLOGINFO
        {
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x3B4)]
            public int Count;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x3D4)] 
            public int ArrayStart;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x3D8)]
            public int ArrayCurrent;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0x3E4)]
            public int LogStart;
        };
        public class ChatLogEntry
        {
            DateTime Timestamp;
            int code;
            string text;
        };
        public Chatlog(CHATLOGINFO _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }

        private void updateOffsetArray()
        {
            offsets_list.Clear();
            MemoryReader mr = MemoryReader.getInstance();
            for (int i = 0; i < Constants.CHATLOG_ARRAY_SIZE; i++)
            {
                offsets_list.Add((int)mr.ResolvePointer((IntPtr)(this.structure.ArrayStart + (i * 0x4))));
            }
        }

        private string ReadEntry(int start, int end)
        {
            int bytesread;

            string ret = Encoding.UTF8.GetString(mr.ReadAdress((IntPtr)(structure.LogStart + start), (uint)(end - start), out bytesread));
            return ret;
        }

        private List<string> ReadEntries(int start, int end)
        {
            var ret = new List<string>();
            for (int i = start; i < end; i++)
            {
                currentOffset = offsets_list[i];
                ret.Add(ReadEntry(previousOffset, currentOffset).Replace("\0", string.Empty));
                previousOffset = currentOffset;
            }
            return ret;
        }

        public List<string> getChatLogStrings()
        {
            updateOffsetArray();
            var ret = new List<string>();
            structure = mr.CreateStructFromAddress<CHATLOGINFO>(address);
            int currentArrayIndex = (structure.ArrayCurrent - structure.ArrayStart) / 4;

            // I forgot why we did this
            if (currentArrayIndex < previousArrayIndex)
            {
                ret.AddRange(ReadEntries(previousArrayIndex, (int)Constants.CHATLOG_ARRAY_SIZE));
                previousOffset = 0;
                previousArrayIndex = 0;
            }
            if (previousArrayIndex < currentArrayIndex)
                ret.AddRange(ReadEntries(previousArrayIndex, currentArrayIndex));
            previousArrayIndex = currentArrayIndex;
            return ret;
        }
    }
}