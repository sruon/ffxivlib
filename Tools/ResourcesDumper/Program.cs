using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SQLite;
using ffxivlib;

namespace ResourcesDumper
{
    public class Buff
    {
        public short id;
        public string name;
        public string description;
        public Buff(short _id, byte[] raw)
        {
            id = _id;
            List<byte> working_copy = raw.ToList();
            // 0x0A byte separates name from description
            int pos = working_copy.FindIndex(0, item => item == 0x0A);
            // If pos == -1, there is no buff with such ID
            if (pos != -1)
                {
                    // Convert to string
                    name = Encoding.UTF8.GetString(working_copy.ToArray(), 0, pos);
                    // Remove everything up to and including the separator
                    working_copy.RemoveRange(0, pos + 1);
                }
            // If pos == -1, there is no description
            if (pos != -1)
                {
                    // Find \0 to finish description
                    pos = working_copy.FindIndex(0, item => item == 0x00);
                    // Convert to string
                    description = Encoding.UTF8.GetString(working_copy.ToArray(), 0, pos);
                }
        }
        public Buff()
        {}
    }
    class Program
    {
        static void dumpBuffs(ffxivlib.FFXIVLIB instance, Serializer s)
        {
            List<Buff> buffs_list = new List<Buff>();
            // This has to be modified by hand atm, 
            // just change your buff a few times, look for where the description is then find out what references this part of memory. (First byte of name)
            IntPtr pointer_to_buff = (IntPtr)0x12F6BEC8;
            IntPtr final_ptr = (IntPtr)BitConverter.ToInt32(instance.ReadMemory(pointer_to_buff, 4), 0);
            for (short i = 0; i < 370; i++)
            {
                Entity player = instance.getEntityById(0);
                player.modify("Buffs", i);
                // Give time to client to update widget
                Thread.Sleep(50);
                byte[] buff = instance.ReadMemory(final_ptr, 400);
                buffs_list.Add(new Buff(i, buff));
            }
            s.Serialize("Buff.xml", buffs_list, "Buff");
        }
        static void dumpSqlite(string filename)
        {
            Serializer s = new Serializer();
            try
            {
                var db = new SQLiteDatabase(filename);
                DataTable recipe;
                List <String> table_list = db.getTables();
                List<SerializableDictionary<string, string>> dict_list = new List<SerializableDictionary<string, string>>();
                foreach (string table in table_list)
                    {
                        String query = string.Format("select * from {0};", table);
                        recipe = db.GetDataTable(query);
                        foreach (DataRow r in recipe.Rows)
                            {
                                SerializableDictionary<string, string> item = new SerializableDictionary<string, string>();
                                foreach (DataColumn c in recipe.Columns)
                                    {
                                        item[c.ToString()] = r[c.ToString()].ToString();
                                    }
                                dict_list.Add(item); 
                            }
                        s.Serialize(string.Format("{0}.xml", table), dict_list, table);
                        dict_list.Clear();
                    }

            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
            }


        }
        static void Main(string[] args)
        {
            Serializer s = new Serializer();
            FFXIVLIB instance = new FFXIVLIB();
            //dumpBuffs(instance, s);
            dumpSqlite("app_data.sqlite");
        }
    }
}
