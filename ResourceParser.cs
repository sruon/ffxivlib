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
        private static readonly Dictionary<string, Dictionary<int, string>> Cache =
            new Dictionary<string, Dictionary<int, string>>();

        /// <summary>
        /// Generator to stream the XML.
        /// </summary>
        /// <param name="fileName">Filename to be streamed</param>
        /// <param name="elementName">The name of a node</param>
        /// <returns>Yields next element</returns>
        private static IEnumerable<XElement> StreamElements(string fileName, string elementName)
        {
            using (var sr = new StreamReader(fileName, Encoding.GetEncoding("UTF-8")))
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
        /// <param name="lookupKey">On what field to match</param>
        /// <param name="resultKey">What field do we need to get back</param>
        /// <param name="search">The search parameter</param>
        /// <returns>The key (ID) or -1</returns>
        private static int RunLinqQuery(string filename, string node, string lookupKey, string resultKey, string search)
        {
            if (search == "")
                return -1;
            if (Cache.ContainsKey(node))
                {
                    foreach (KeyValuePair<int, string> pair in Cache[node])
                        {
                            if (pair.Value == search)
                                return pair.Key;
                        }
                }
            else
                Cache.Add(node, new Dictionary<int, string>());
            var result = (from item in StreamElements(filename, node)
                where item.Element(lookupKey).Value == search
                select item.Element(resultKey).Value).FirstOrDefault();
            int key = int.Parse(result);
            if (key > 0)
                Cache[node][key] = search;
            return key;
        }

        /// <summary>
        /// Attempts to retrieve the value in internal cache or run the Linq query
        /// The value is then added to the cache.
        /// </summary>
        /// <param name="filename">Resource file to parse</param>
        /// <param name="node">Type of resource</param>
        /// <param name="lookupKey">On what field to match</param>
        /// <param name="resultKey">What field do we need to get back</param>
        /// <param name="search">The search parameter</param>
        /// <returns>The value (string) or string.empty</returns>
        private static string RunLinqQuery(string filename, string node, string lookupKey, string resultKey, int search)
        {
            if (search == 0)
                return string.Empty;
            if (Cache.ContainsKey(node))
                {
                    if (Cache[node].ContainsKey(search))
                        return Cache[node][search];
                }
            else
                Cache.Add(node, new Dictionary<int, string>());
            var result = (from item in StreamElements(filename, node)
                where int.Parse(item.Element(lookupKey).Value) == search
                select item.Element(resultKey).Value).FirstOrDefault();
            if (result != null)
                Cache[node][search] = result;
            return result;
        }

        /// <summary>
        /// Attempts to retrieve the value in internal cache or run the Linq query
        /// The value is then added to the cache.
        /// This needs to be reworked to handle multiple search parameters easily.
        /// </summary>
        /// <param name="filename">Resource file to parse</param>
        /// <param name="node">Type of resource</param>
        /// <param name="lookupKeys"></param>
        /// <param name="resultKey">What field do we need to get back</param>
        /// <param name="searchValues"></param>
        /// <returns>The value (string) or string.empty</returns>
        private static string RunLinqQuery(string filename, string node, string[] lookupKeys, string resultKey,
            int[] searchValues)
        {
            if (searchValues.First() == 0)
                return string.Empty;
            if (Cache.ContainsKey(node))
                {
                    int key = int.Parse(string.Format("{0}{1}", searchValues.First(), searchValues.Last()));
                    if (Cache[node].ContainsKey(key))
                        return Cache[node][key];
                }
            else
                Cache.Add(node, new Dictionary<int, string>());
            var result = (from item in StreamElements(filename, node)
                where int.Parse(item.Element(lookupKeys[0]).Value) == searchValues[0] &&
                      int.Parse(item.Element(lookupKeys[1]).Value) == searchValues[1]
                select item.Element(resultKey).Value).FirstOrDefault();
            if (result != null)
                {
                    int key = int.Parse(string.Format("{0}{1}", searchValues.First(), searchValues.Last()));
                    Cache[node][key] = result;
                }
            return result;
        }

        #region Autotranslate lookups

        /// <summary>
        /// Retrieves Autotranslate corresponding to ID
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>Item Name</returns>
        public static string GetAutotranslate(int CategoryId, int Id)
        {
            string[] lookupKeys = {"CategoryId", "Id"};
            int[] searchValues = {CategoryId, Id};

            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.AUTOTRANSLATE_FILE), "Autotranslate", lookupKeys, "Content",
                    searchValues);
        }

        #endregion

        #region Zone lookups

        /// <summary>
        /// Retrieves Zone Name corresponding to ID.
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <returns>Zone name</returns>
        public static string GetZoneName(int id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.ZONE_FILE), "PlaceName", "Key",
                    "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        /// <summary>
        /// Retrieves Zone ID corresponding to Name.
        /// </summary>
        /// <param name="name">Zone name</param>
        /// <returns>Zone ID</returns>
        public static int GetZoneID(string name)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.ZONE_FILE), "PlaceName",
                    "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, "Key", name);
        }

        #endregion

        #region Item lookups

        /// <summary>
        /// Retrieves Item name corresponding to ID
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>Item Name</returns>
        public static string GetItemName(int id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.ITEM_FILE), "Item", "Key",
                    "UIName_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        /// <summary>
        /// Retrieves Item name corresponding to ID in ITEM structure
        /// </summary>
        /// <param name="item">Item structure</param>
        /// <returns>Item Name</returns>
        public static string GetItemName(Inventory.ITEM item)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.ITEM_FILE), "Item", "Key",
                    "UIName_" + Constants.ResourceParser.RESOURCES_LANGUAGE, (int) item.ItemID);
        }

        /// <summary>
        /// Retrieves Item ID corresponding to name
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <returns>Item ID</returns>
        public static int GetItemID(string name)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.ITEM_FILE), "Item",
                    "UIName_" + Constants.ResourceParser.RESOURCES_LANGUAGE, "Key", name);
        }

        #endregion

        #region Buff lookups

        /// <summary>
        /// Retrieves Buff name corresponding to ID
        /// </summary>
        /// <param name="id">Buff ID</param>
        /// <returns>Buff Name</returns>
        public static string GetBuffName(short id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.BUFF_FILE), "Buff", "id", "name", id);
        }

        /// <summary>
        /// Retrieves Buff name corresponding to ID in BUFF structure
        /// </summary>
        /// <returns>Buff Name</returns>
        public static string GetBuffName(BUFF item)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.BUFF_FILE), "Buff", "id", "name", item.BuffID);
        }

        /// <summary>
        /// Retrieves Buff ID corresponding to name.
        /// </summary>
        /// <param name="name">Buff name</param>
        /// <returns>Buff ID</returns>
        public static int GetBuffID(string name)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.BUFF_FILE), "Buff", "name", "id", name);
        }

        #endregion

        #region Title lookups

        /// <summary>
        /// Retrieves Title Name corresponding to ID.
        /// </summary>
        /// <param name="id">Title ID</param>
        /// <returns>Title name</returns>
        public static string GetTitleName(byte id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.TITLE_FILE), "Title", "Key",
                    "Male_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        /// <summary>
        /// Retrieves Title Name corresponding to ID in Entity.
        /// </summary>
        /// <param name="e">Entity</param>
        /// <returns>Title name</returns>
        public static string GetTitleName(Entity e)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.TITLE_FILE), "Title", "Key",
                    "Male_" + Constants.ResourceParser.RESOURCES_LANGUAGE, e.Structure.Title);
        }

        #endregion

        #region Job lookups

        /// <summary>
        /// Retrieves Job Name corresponding to ID.
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>Job name</returns>
        public static string GetJobName(JOB id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.JOB_FILE), "ClassJob", "Key",
                    "Name_" + Constants.ResourceParser.RESOURCES_LANGUAGE, (byte) id);
        }

        /// <summary>
        /// Retrieves Job Name corresponding to ID in Entity.
        /// </summary>
        /// <param name="e">Entity</param>
        /// <returns>Job name</returns>
        public static string GetJobName(Entity e)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.JOB_FILE), "ClassJob", "Key",
                    "Name_" + Constants.ResourceParser.RESOURCES_LANGUAGE, (byte) e.Job);
        }

        /// <summary>
        /// Retrieves Job shortname corresponding to ID.
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>Job shortname</returns>
        public static string GetJobShortname(JOB id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.JOB_FILE), "ClassJob", "Key",
                    "Abbreviation_" + Constants.ResourceParser.RESOURCES_LANGUAGE, (byte) id);
        }

        /// <summary>
        /// Retrieves Job shortname corresponding to ID in Entity.
        /// </summary>
        /// <param name="e">Entity</param>
        /// <returns>Job shortname</returns>
        public static string GetJobShortname(Entity e)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.JOB_FILE), "ClassJob", "Key",
                    "Abbreviation_" + Constants.ResourceParser.RESOURCES_LANGUAGE, (byte) e.Job);
        }

        /// <summary>
        /// Retrieves Job ID corresponding to shortname.
        /// </summary>
        /// <param name="name">Job shortname</param>
        /// <returns>Job ID</returns>
        public static int GetJobID(string name)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.JOB_FILE), "ClassJob",
                    "Abbreviation_" + Constants.ResourceParser.RESOURCES_LANGUAGE, "Key", name);
        }

        #endregion

        #region Grand Company lookups

        /// <summary>
        /// Retrieves Grand Company name corresponding to ID
        /// </summary>
        /// <param name="id">GC ID</param>
        /// <returns>GC Name</returns>
        public static string GetGrandCompany(byte id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.GRAND_COMPANY_FILE), "GrandCompany", "Key",
                    "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        /// <summary>
        /// Retrieves Grand Company name corresponding to ID in Entity
        /// </summary>
        /// <param name="e">Entity</param>
        /// <returns>GC Name</returns>
        public static string GetGrandCompany(Entity e)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.GRAND_COMPANY_FILE), "GrandCompany", "Key",
                    "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, e.Structure.GrandCompany);
        }

        /// <summary>
        /// Retrieves Grand Company rank corresponding to ID
        /// Remarks: This returns only Uldah ranks at the moment
        /// </summary>
        /// <param name="id">GC Rank ID</param>
        /// <returns>GC Rank Name</returns>
        public static string GetGrandCompanyRank(byte id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.GRAND_COMPANY_RANK_FILE), "GCRankUldahMaleText", "Key",
                    "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        /// <summary>
        /// Retrieves Grand Company rank corresponding to ID in Entity
        /// Remarks: This returns only Uldah ranks at the moment
        /// </summary>
        /// <param name="e">Entity</param>
        /// <returns>GC Rank Name</returns>
        public static string GetGrandCompanyRank(Entity e)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.GRAND_COMPANY_RANK_FILE), "GCRankUldahMaleText", "Key",
                    "SGL_" + Constants.ResourceParser.RESOURCES_LANGUAGE, e.Structure.GrandCompanyRank);
        }

        #endregion

        #region Quests lookups

        /// <summary>
        /// Retrieves Quest Name corresponding to ID.
        /// </summary>
        /// <param name="id">Quest ID</param>
        /// <returns>Quest name</returns>
        public static string GetQuestName(int id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.QUEST_FILE), "Quest", "Key",
                    "Name_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        #endregion

        #region Actions lookups

        /// <summary>
        /// Retrieves Action name corresponding to ID
        /// </summary>
        /// <param name="id">Action ID</param>
        /// <returns>Action Name</returns>
        public static string GetActionName(int id)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.ACTION_FILE), "Action", "Key",
                    "Name_" + Constants.ResourceParser.RESOURCES_LANGUAGE, id);
        }

        /// <summary>
        /// Retrieves Action name corresponding to ID
        /// </summary>
        /// <param name="id">Action ID</param>
        /// <returns>Action Name</returns>
        public static string GetActionName(short id)
        {
            return GetActionName((int)id);
        }

        /// <summary>
        /// Retrieves Action ID corresponding to name
        /// </summary>
        /// <param name="name">Action Name</param>
        /// <returns>Action ID</returns>
        public static int GetActionID(string name)
        {
            return
                RunLinqQuery(
                    string.Format("{0}/{1}", Constants.ResourceParser.RESOURCES_FOLDER,
                        Constants.ResourceParser.ACTION_FILE), "Action",
                    "Name_" + Constants.ResourceParser.RESOURCES_LANGUAGE, "Key", name);
        }

        #endregion
    }
}