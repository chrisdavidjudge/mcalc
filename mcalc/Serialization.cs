using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace mcalc.XML
{
    internal static class Serialization
    {
        public static void Serialize(FileStream stream, object obj)
        {
            XmlSerializer xml = new XmlSerializer(obj.GetType());
            xml.Serialize(stream, obj);
        }

        public static void Serialize(TextWriter stream, object obj)
        {
            XmlSerializer xml = new XmlSerializer(obj.GetType());
            xml.Serialize(stream, obj);
        }

        public static object Deserialize(FileStream stream, Type type)
        {
            XmlSerializer xml = new XmlSerializer(type);
            return xml.Deserialize(stream);
        }
        public static object Deserialize(string inputUri, Type type)
        {
            using XmlReader reader = XmlReader.Create(inputUri);
            XmlSerializer xml = new XmlSerializer(type);
            return xml.Deserialize(reader);
        }
    }
}
