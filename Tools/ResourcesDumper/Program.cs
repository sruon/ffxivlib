using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using ffxivlib;

namespace ResourcesDumper
{
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
    class Program
    {
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
            //FFXIVLIB instance = new FFXIVLIB();
            //dumpBuffs(instance, s);
            DumpSqlite("app_data.sqlite");
        }
    }
}
