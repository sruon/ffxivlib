using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ffxivlib
{
    public static class ResourceParser
    {
        private static Dictionary<string, Dictionary<int, string>> Cache = new Dictionary<string, Dictionary<int, string>>(); 
        
        /// <summary>
        /// Generator to stream the XML.
        /// </summary>
        /// <param name="fileName">Filename to be streamed</param>
        /// <param name="elementName">The name of a node</param>
        /// <returns>Yields next element</returns>
        private static IEnumerable<XElement> StreamElements(string fileName, string elementName)
        {
            using (StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding("UTF-8")))
                {
                    using (var rdr = XmlTextReader.Create(sr))
                        {
                            rdr.MoveToContent();
                            while (rdr.Read())
                                {
                                    if ((rdr.NodeType == XmlNodeType.Element) && (rdr.Name == elementName))
                                        {
                                            var e = XElement.ReadFrom(rdr) as XElement;
                                            yield return e;
                                        }
                                }
                            rdr.Close();
                        }
                    sr.Close();
                }
        }

        /// <summary>
        /// Attempts to retrieve the key in internal cache (we iterate over the Dictionary 
        /// as it is less costly than running XML parsing again and again)
        /// or run the Linq query
        /// The value is then added to the cache.
        /// </summary>
        /// <param name="filename">Resource file to parse</param>
        /// <param name="node">Type of resource</param>
        /// <param name="lookup_key">On what field to match</param>
        /// <param name="result_key">What field do we need to get back</param>
        /// <param name="search">The search parameter</param>
        /// <returns>The key (ID) or -1</returns>
        public static int runLinqQuery(string filename, string node, string lookup_key, string result_key, string search)
        {
            if (search == "")
                return -1;
            if (Cache.ContainsKey(node))
            {
                foreach (KeyValuePair<int, string> pair in Cache[node])
                {
                    if (pair.Value == search)
                        {
                            return pair.Key;
                        }
                }
            }
            else
            {
                Cache.Add(node, new Dictionary<int, string>());
            }
            var result = (from item in StreamElements(filename, node)
                          where item.Element(lookup_key).Value == search
                          select item.Element(result_key).Value).FirstOrDefault();
            int key = int.Parse(result);
            if (key > 0)
                {
                    Cache[node][key] = search;
                }
            return key;
        }

        /// <summary>
        /// Attempts to retrieve the value in internal cache or run the Linq query
        /// The value is then added to the cache.
        /// </summary>
        /// <param name="filename">Resource file to parse</param>
        /// <param name="node">Type of resource</param>
        /// <param name="lookup_key">On what field to match</param>
        /// <param name="result_key">What field do we need to get back</param>
        /// <param name="search">The search parameter</param>
        /// <returns>The value (string) or string.empty</returns>
        public static string runLinqQuery(string filename, string node, string lookup_key, string result_key, int search)
        {
            if (search == 0)
                return string.Empty;
            if (Cache.ContainsKey(node))
                {
                    if (Cache[node].ContainsKey(search))
                        {
                            return Cache[node][search];
                        }
                }
            else
                {
                    Cache.Add(node, new Dictionary<int, string>());
                }
            var result = (from item in StreamElements(filename, node)
                          where int.Parse(item.Element(lookup_key).Value) == search
                          select item.Element(result_key).Value).FirstOrDefault();
            if (result != null)
                {
                    Cache[node][search] = result;
                }
            return result;
        }

        #region Item lookups
        public static string getItemName(int id)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.ITEM_FILE), "Item", "Key", "UIName_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        public static string getItemName(Inventory.ITEM item)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.ITEM_FILE), "Item", "Key", "UIName_" + Constants.ResourceParser.RESOURCES_LANGUAGE, (int)item.ItemID);
        }

        public static int getItemID(string name)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.ITEM_FILE), "Item", "UIName_" + Constants.ResourceParser.RESOURCES_LANGUAGE, "Key", name);
        }
        #endregion

        #region Buff lookups
        public static string getBuffName(short id)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.BUFF_FILE), "Buff", "id", "name", id);
        }

        public static string getBuffName(BUFF item)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.BUFF_FILE), "Buff", "id", "name", item.BuffID);
        }

        public static int getBuffID(string name)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.BUFF_FILE), "Buff", "name", "id", name);
        }
        #endregion

        #region Title lookups

        public static string getTitleName(byte id)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.TITLE_FILE), "Title", "Key", "Male_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        public static string getTitleName(Entity e)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.TITLE_FILE), "Title", "Key", "Male_" + Constants.ResourceParser.RESOURCES_LANGUAGE, e.structure.Title);
        }
        #endregion

        #region Grand Company lookups

        public static string getGrandCompany(byte id)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.GRAND_COMPANY_FILE), "GrandCompany", "Key", "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        public static string getGrandCompany(Entity e)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.GRAND_COMPANY_FILE), "GrandCompany", "Key", "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, e.structure.GrandCompany);
        }

        public static string getGrandCompanyRank(byte id)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.GRAND_COMPANY_RANK_FILE), "GCRankUldahMaleText", "Key", "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        public static string getGrandCompanyRank(Entity e)
        {
            return runLinqQuery(string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER, Constants.ResourceParser.GRAND_COMPANY_RANK_FILE), "GCRankUldahMaleText", "Key", "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, e.structure.GrandCompanyRank);
        }

        #endregion
    }
}
