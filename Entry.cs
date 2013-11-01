using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ffxivlib
{
    public partial class Chatlog
    {
        public class Entry
        {
            public byte[] raw;

            /// <summary>
            ///     Builds a chatlog entry out of a byte array
            ///     The implementation is sketchy at best but it should be reliable enough
            /// </summary>
            /// <param name="_raw">The array to process</param>
            public Entry(byte[] _raw)
            {
                raw = _raw;
                raw_string = Encoding.UTF8.GetString(_raw);
                processEntry(_raw);
            }

            public DateTime timestamp { get; set; }
            public string code { get; set; }
            public string text { get; set; }
            public string raw_string { get; set; }

            /// <summary>
            ///     Main processing function
            ///     This extracts the timestamp, code and process the text to clean it.
            /// </summary>
            /// <param name="raw">The array to process</param>
            private void processEntry(byte[] raw)
            {
                List<byte> working_copy = raw.ToList();
                if (raw.Length < Constants.TIMESTAMP_SIZE + Constants.CHATCODE_SIZE)
                    return;
                try
                {
                    timestamp = getTimeStamp(int.Parse(
                        Encoding.UTF8.GetString(
                            working_copy.ToArray()
                            ).Substring(0, Constants.TIMESTAMP_SIZE),
                        NumberStyles.HexNumber));
                }
                catch
                {
                    return;
                }
                working_copy.RemoveRange(0, 8);
                code = Encoding.UTF8.GetString(working_copy.ToArray(), 0, Constants.CHATCODE_SIZE);
                working_copy.RemoveRange(0, 4);
                int sep = working_copy[1] == ':' ? 2 : 1; // Removes :: separators 
                working_copy.RemoveRange(0, sep);
                working_copy = cleanFormat(working_copy);
                working_copy = cleanName(working_copy);
                working_copy = cleanMob(working_copy);
                working_copy = cleanHQ(working_copy);
                text = cleanString(Encoding.UTF8.GetString(working_copy.ToArray()));
            }

            /// <summary>
            ///     Removes any invalid character left
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            private string cleanString(string input)
            {
                return new string(input.Where(value =>
                                              (value >= 0x0020 && value <= 0xD7FF) ||
                                              (value >= 0xE000 && value <= 0xFFFD) ||
                                              value == 0x0009 ||
                                              value == 0x000A ||
                                              value == 0x000D).ToArray());
            }

            /// <summary>
            ///     Removes junk around NPC names that's only useful to the client.
            /// </summary>
            /// <param name="working_copy"></param>
            /// <returns></returns>
            private List<byte> cleanMob(List<byte> working_copy)
            {
                var pattern = new List<byte>
                    {
                        0x20,
                        0x20,
                        0xEE,
                        0x81,
                        0xAF,
                        0x20
                    };
                for (int i = 0; i < pattern.Count; i++)
                {
                    if (working_copy[i] != pattern[i])
                        return working_copy;
                }
                working_copy.RemoveRange(0, pattern.Count);
                return working_copy;
            }

            /// <summary>
            ///     This replaces the HQ icon 0xEE 0x80 0xBC by a simple HQ 0x48 0x51
            /// </summary>
            /// <param name="working_copy"></param>
            /// <returns></returns>
            private List<byte> cleanHQ(List<byte> working_copy)
            {
                var pattern = new List<byte>
                    {
                        0xEE,
                        0x80,
                        0xBC
                    };
                var hq_rep = new List<byte>
                    {
                        0x48,
                        0x51
                    };
                int i = working_copy.FindIndex(item => item == pattern[0]);
                if (i == -1)
                    return working_copy;
                for (; i < pattern.Count; i++)
                {
                    if (working_copy[i] != pattern[i])
                        return working_copy;
                }
                working_copy.RemoveRange(i, pattern.Count);
                working_copy.InsertRange(i, hq_rep);
                return working_copy;
            }

            /// <summary>
            ///     Removes tags used for formatting
            ///     0x02 0xXX 0xXX 0x03
            ///     0x02 0xXX 0xXX 0xXX 0xXX 0xXX 0xXX 0x03
            /// </summary>
            /// <param name="working_copy"></param>
            /// <returns></returns>
            private List<byte> cleanFormat(List<byte> working_copy)
            {
                int[] idx = working_copy.Select((b, i) => b == 0x02 ? i : -1).Where(i => i != -1).ToArray();
                bool changed = false;
                foreach (int i in idx)
                {
                    if (working_copy.Count > i + 8 && working_copy[i + 8] == 0x03)
                    {
                        working_copy.RemoveRange(i, 9);
                        changed = true;
                    }
                    if (working_copy.Count > i + 4 && working_copy[i + 4] == 0x03)
                    {
                        working_copy.RemoveRange(i, 5);
                        changed = true;
                    }
                    if (changed)
                    {
                        working_copy = cleanFormat(working_copy);
                    }
                }
                return working_copy;
            }

            /// <summary>
            ///     Removes junk around PC names that's only useful to the client.
            /// </summary>
            /// <param name="working_copy"></param>
            /// <returns></returns>
            private List<byte> cleanName(List<byte> working_copy)
            {
                if (working_copy.Count(f => f == 0x3) == 1)
                    return working_copy;
                int name = working_copy.FindIndex(0, item => item == 0x3);
                if (name != -1)
                {
                    working_copy.RemoveRange(0, name + 1);
                    name = working_copy.FindIndex(0, item => item == 0x3);
                    if (name != -1)
                    {
                        working_copy.RemoveRange(name - 9, 10);
                    }
                }
                return working_copy;
            }

            /// <summary>
            ///     Creates a DateTime object out of our timestamp
            /// </summary>
            /// <param name="value">Timestamp to convert</param>
            /// <returns>DateTime object corresponding to the timestamp</returns>
            private DateTime getTimeStamp(double value)
            {
                var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return dt.AddSeconds(value).ToLocalTime();
            }
        };
    }
}