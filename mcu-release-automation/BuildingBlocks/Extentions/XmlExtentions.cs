using System.Xml;
using System.Xml.Linq;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public static class XmlExtentions
    {
        public static XElement GetXElement(this XmlNode node)
        {
            var xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        public static XmlNode GetXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }
    }
}
