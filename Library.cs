using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ffxivlib
{
    internal class Library
    {
        internal static int ByteSearch(byte[] haystack, byte[] needle, int start = 0)
        {
            int found = -1;
            if (haystack.Length > 0 && needle.Length > 0 && start <= (haystack.Length - needle.Length) && haystack.Length >= needle.Length)
                {
                    for (int i = start; i <= haystack.Length - needle.Length; i++)
                        {
                            if (haystack[i] == needle[0])
                                {
                                    if (haystack.Length > 1)
                                        {
                                            bool matched = true;
                                            for (int y = 1; y <= needle.Length - 1; y++)
                                                {
                                                    if (haystack[i + y] != needle[y])
                                                        {
                                                            matched = false;
                                                            break;
                                                        }
                                                }
                                            if (!matched) continue;
                                            found = i;
                                            break;
                                        }
                                    found = i;
                                    break;
                                }
                        }

                }
            return found;
        }
    }
}
