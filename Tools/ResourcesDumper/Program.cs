using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using ffxivlib;

namespace ResourcesDumper
{
    public class ATCategory
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int First { get; set; }
    }

    public class Buff
    {
        public short ID;
        public string Name;
        public string Description;
        public Buff(short id, IEnumerable<byte> raw)
        {
            ID = id;
            List<byte> workingCopy = raw.ToList();
            // 0x0A byte separates name from description
            int pos = workingCopy.FindIndex(0, item => item == 0x0A);
            // If pos == -1, there is no buff with such ID
            if (pos != -1)
                {
                    // Convert to string
                    Name = Encoding.UTF8.GetString(workingCopy.ToArray(), 0, pos);
                    // Remove everything up to and including the separator
                    workingCopy.RemoveRange(0, pos + 1);
                }
            // If pos == -1, there is no description
            if (pos != -1)
                {
                    // Find \0 to finish description
                    pos = workingCopy.FindIndex(0, item => item == 0x00);
                    // Convert to string
                    Description = Encoding.UTF8.GetString(workingCopy.ToArray(), 0, pos);
                }
        }
        public Buff()
        {}
    }

    public class Autotranslate
    {
        public int Id;
        public int CategoryId;
        public string Content;

        public Autotranslate(int _CategoryId, int _Id, string _Content)
        {
            Id = _Id;
            Content = _Content;
            CategoryId = _CategoryId;
        }
        public Autotranslate()
        { }
    }
    class Program
    {
        private static ATCategory[] categories =
        {
            new ATCategory {Id = 1, First = 102, Name = "Languages"},
            new ATCategory {Id = 2, First = 202, Name = "Greetings"},
            new ATCategory {Id = 3, First = 301, Name = "Questions"},
            new ATCategory {Id = 4, First = 401, Name = "Answers"},
            new ATCategory {Id = 5, First = 501, Name = "Reasons"},
            new ATCategory {Id = 6, First = 601, Name = "Trade"},
            new ATCategory {Id = 7, First = 701, Name = "Organize"},
            new ATCategory {Id = 8, First = 801, Name = "Tactics"},
            new ATCategory {Id = 9, First = 901, Name = "Roles"},
            new ATCategory {Id = 10, First = 951, Name = "Locations"},
            new ATCategory {Id = 11, First = 1001, Name = "Time"},
            new ATCategory {Id = 13, First = 1151, Name = "Transportation"},
            new ATCategory {Id = 14, First = 1201, Name = "Facilities"},
            new ATCategory {Id = 15, First = 1251, Name = "Grand Company"},
            new ATCategory {Id = 16, First = 1271, Name = "Free Company"},
            new ATCategory {Id = 17, First = 1301, Name = "System"},
            new ATCategory {Id = 18, First = 1401, Name = "Battle"},
            new ATCategory {Id = 19, First = 1501, Name = "Online Status"},
            new ATCategory {Id = 0x14, First = 1551, Name = "Communication"},
            new ATCategory {Id = 0x16, First = 1601, Name = "Crafting & Gathering"},
            new ATCategory {Id = 0x17, First = 1901, Name = "Primals"},
            new ATCategory {Id = 0x18, First = 2001, Name = "Duty"},
            new ATCategory {Id = 0x32, First = 2, Name = "Classes & Jobs"},
            new ATCategory {Id = 0x33, First = 21, Name = "Places"},
            new ATCategory {Id = 0x34, First = 2, Name = "Races"},
            new ATCategory {Id = 0x35, First = 2, Name = "Clans"},
            new ATCategory {Id = 0x36, First = 2, Name = "Guardians"},
            new ATCategory {Id = 0x37, First = 2, Name = "General Actions"},
            new ATCategory {Id = 0x38, First = 10, Name = "Disciple of War & Magic actions"},
            new ATCategory {Id = 0x39, First = 100001, Name = "Disciple of the Hand actions"},
            new ATCategory {Id = 0x3A, First = 3, Name = "Companion actions"},
            new ATCategory {Id = 0x3B, First = 2, Name = "Pet actions"},
            new ATCategory {Id = 0x3C, First = 2, Name = "Weather"},
            new ATCategory {Id = 0x3D, First = 2, Name = "Main commands"},
            new ATCategory {Id = 0x3E, First = 103, Name = "Text commands"},
        };
        /// <summary>
        /// Master array -> Instance 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="s"></param>
        static void DumpAutoTranslate(FFXIVLIB instance, Serializer s)
        {
            List<Autotranslate> atlist = new List<Autotranslate>();
            IntPtr pointerInMasterArray = (IntPtr)0x4494280;
            IntPtr finalPTR = (IntPtr)BitConverter.ToInt32(instance.ReadMemory(pointerInMasterArray, 4), 0);
            int categoryiter = 0;
            while (finalPTR != IntPtr.Zero)
                {
                    IntPtr objectPtr = (IntPtr)BitConverter.ToInt32(instance.ReadMemory(finalPTR + 4, 4), 0);
                    IntPtr addressOfArray = (IntPtr)BitConverter.ToInt32(instance.ReadMemory(objectPtr, 4), 0);
                    int itemId = 0;
                    while (addressOfArray != IntPtr.Zero)
                        {
                            
                            byte[] memrep = instance.ReadMemory(addressOfArray, 128);
                            byte[] category = instance.ReadMemory(addressOfArray - 5, 1);
                            List<byte> workingCopy = memrep.ToList();
                            int name = workingCopy.FindIndex(0, item => item == 0x00);
                            if (name != -1)
                                {
                                    workingCopy.RemoveRange(name + 1, workingCopy.Count - name - 1);
                                }
                            string rep = System.Text.Encoding.UTF8.GetString(workingCopy.ToArray());
                            rep = rep.Replace("\0", string.Empty);
                            atlist.Add(new Autotranslate(categories[categoryiter].Id, categories[categoryiter].First + itemId, rep));
                            itemId += 1;
                            addressOfArray = (IntPtr)BitConverter.ToInt32(instance.ReadMemory(objectPtr + (itemId * 4), 4), 0);
                        }

                    // Move in master array
                    pointerInMasterArray += 0x4;
                    finalPTR = (IntPtr)BitConverter.ToInt32(instance.ReadMemory(pointerInMasterArray, 4), 0);
                    categoryiter += 1;
                }
            s.Serialize("Autotranslate.xml", atlist, "Autotranslate");
        }

        static void DumpBuffs(FFXIVLIB instance, Serializer s)
        {
            List<Buff> buffsList = new List<Buff>();
            // This has to be modified by hand atm, 
            // just change your buff a few times, look for where the description is then find out what references this part of memory. (First byte of name)
            IntPtr pointerToBuff = (IntPtr)0x12F6BEC8;
            IntPtr finalPTR = (IntPtr)BitConverter.ToInt32(instance.ReadMemory(pointerToBuff, 4), 0);
            for (short i = 0; i < 370; i++)
            {
                Entity player = instance.GetEntityById(0);
                player.Modify("Buffs", i);
                // Give time to client to update widget
                Thread.Sleep(50);
                byte[] buff = instance.ReadMemory(finalPTR, 400);
                buffsList.Add(new Buff(i, buff));
            }
            s.Serialize("Buff.xml", buffsList, "Buff");
        }

        static void DumpSqlite(string filename)
        {
            Serializer s = new Serializer();
            try
            {
                var db = new SQLiteDatabase(filename);
                List <String> tableList = db.GetTables();
                List<SerializableDictionary<string, string>> dictList = new List<SerializableDictionary<string, string>>();
                foreach (string table in tableList)
                    {
                        String query = string.Format("select * from {0};", table);
                        DataTable recipe = db.GetDataTable(query);
                        foreach (DataRow r in recipe.Rows)
                            {
                                SerializableDictionary<string, string> item = new SerializableDictionary<string, string>();
                                foreach (DataColumn c in recipe.Columns)
                                    {
                                        item[c.ToString()] = r[c.ToString()].ToString();
                                    }
                                dictList.Add(item); 
                            }
                        s.Serialize(string.Format("{0}.xml", table), dictList, table);
                        dictList.Clear();
                    }

            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message + "\n\n";
            }


        }
        static void Main(string[] args)
        {
            Serializer s = new Serializer();
            FFXIVLIB instance = new FFXIVLIB();
            //dumpBuffs(instance, s);
            //DumpSqlite("app_data.sqlite");
            DumpAutoTranslate(instance, s);
        }
    }
}
