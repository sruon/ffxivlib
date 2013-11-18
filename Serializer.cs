using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ffxivlib
{
    internal class Serializer
    {
        /// <summary>
        /// Serializes a list of type T to the filename given.
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="filename">Filename to save the XML output</param>
        /// <param name="list">List of instances</param>
        /// <returns>Filename or null on failure</returns>
        public string Serialize<T>(string filename, T list)
        {
            if (filename == "")
                return null;
            try
                {
                    var serializer = new XmlSerializer(typeof (T));
                    TextWriter textWriter = new StreamWriter(filename);
                    serializer.Serialize(textWriter, list);
                    textWriter.Close();
                    return filename;
                }
            catch (IOException ex)
                {
                    Debug.WriteLine("Serialize failed with IOException: {0}", ex.Message);
                }
            catch (Exception ex)
                {
                    Debug.WriteLine("Serialize failed with Exception: {0}", ex.Message);
                }
            return null;
        }

        /// <summary>
        /// Deserializes a XML file into an object of type T (usually a list)
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize into</typeparam>
        /// <param name="filename">Filename to read XML from</param>
        /// <returns>Deserialized instances or default(T) which should be null on failure</returns>
        public T Deserialize<T>(string filename)
        {
            if (filename == "")
                return default(T);
            try
                {
                    var deserializer = new XmlSerializer(typeof (T));
                    TextReader textReader = new StreamReader(filename);
                    var desList = (T) deserializer.Deserialize(textReader);
                    textReader.Close();
                    return desList;
                }
            catch (IOException ex)
                {
                    Debug.WriteLine("Serialize failed with IOException: {0}", ex.Message);
                }
            catch (Exception ex)
                {
                    Debug.WriteLine("Serialize failed with Exception: {0}", ex.Message);
                }
            return default(T);
        }
    }
}