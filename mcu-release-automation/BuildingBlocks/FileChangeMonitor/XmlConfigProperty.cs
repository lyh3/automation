using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace McAfeeLabs.Engineering.Automation.Base
{
    abstract public class XmlConfigProperty : FileContentProperty
    {
        public const string AppSettingsKeyXpathFormat = @"/configuration/appSettings/add[@key='{0}']/@value";
        string[] ExtensionFilters = { ".xml", ".config" };
        private static object _syncObj = new object();
        public string XPath { get; set; }

        public override void Sync(string filefullpath)
        {
            lock (_syncObj)
            {
                var extension = Path.GetExtension(filefullpath).ToLower();
                var extensionlist = new List<string>();
                extensionlist.AddRange(ExtensionFilters);

                if (null == extensionlist.FirstOrDefault<string>(x => x == extension)) return;

                var xmldoc = new XmlDocument();
                xmldoc.Load(filefullpath);
                var xmlnode = xmldoc.SelectSingleNode(XPath);
                Value = Convert(xmlnode);
            }
        }

        abstract protected dynamic Convert(XmlNode node);
    }

    public class StringConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            return node.InnerText;
        }
    }

    public class IntConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            var value = -1;
            int.TryParse(node.InnerText, out value);
            return value;
        }
    }

    public class LongConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            var value = -1L;
            long.TryParse(node.InnerText, out value);
            return value;
        }
    }

    public class DoubleConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            var value = 0.0;
            double.TryParse(node.InnerText, out value);
            return value;
        }
    }

    public class FloatConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            var value = 0.0f;
            float.TryParse(node.InnerText, out value);
            return value;
        }
    }

    public class Int64ConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            var value = (Int64)0;
            Int64.TryParse(node.InnerText, out value);
            return value;
        }
    }

    public class BooleanConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            var value = false;
            bool.TryParse(node.InnerText, out value);
            return value;
        }
    }

    public class XmlDocConfigProperty : XmlConfigProperty
    {
        override protected dynamic Convert(XmlNode node)
        {
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(node.OuterXml);
            return xmldoc;
        }
    }
}
