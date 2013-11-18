using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

[XmlRoot("dictionary")]
public class SerializableDictionary<TKey, TValue>
    : Dictionary<TKey, TValue>, IXmlSerializable
{
    #region IXmlSerializable Members

    public XmlSchema GetSchema()
    {
        return null;
    }


    public void ReadXml(XmlReader reader)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof (TKey));

        XmlSerializer valueSerializer = new XmlSerializer(typeof (TValue));


        bool wasEmpty = reader.IsEmptyElement;

        reader.Read();


        if (wasEmpty)

            return;


        while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");


                reader.ReadStartElement("key");

                TKey key = (TKey) keySerializer.Deserialize(reader);

                reader.ReadEndElement();


                reader.ReadStartElement("value");

                TValue value = (TValue) valueSerializer.Deserialize(reader);

                reader.ReadEndElement();


                Add(key, value);


                reader.ReadEndElement();

                reader.MoveToContent();
            }

        reader.ReadEndElement();
    }


    public void WriteXml(XmlWriter writer)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof (TKey));

        XmlSerializer valueSerializer = new XmlSerializer(typeof (TValue));

        writer.WriteStartElement("XMLNODE");
        foreach (TKey key in Keys)
            {
                TValue value = this[key];
                writer.WriteElementString(key.ToString(), value.ToString());
            }
        writer.WriteEndElement();
    }

    #endregion
}