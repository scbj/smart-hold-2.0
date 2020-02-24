using Smart_Hold.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Smart_Hold.Xml
{
    public class XmlConfig : IXmlConfig
    {
        // Properties
        public string Filename { get; set; }
        public XmlDocument Document { get; set; }
        public XmlNode Root { get; set; }


        // Constructors
        public XmlConfig(string filename)
        {
            Filename = filename;
        }


        // Methods
        public void Initialize()
        {
            try
            {
                Document = new XmlDocument();

                if (File.Exists(Filename))
                    Document.Load(Filename);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void Clear() => File.Delete(Filename);

        public void SetRoot(string root)
        {
            if (Document.SelectNodes(root).Count > 0)
                Root = Document.SelectSingleNode(root);
            else
            {
                Root = Document.CreateElement(root.Substring(2));
                Document.AppendChild(Root);
            }
        }

        public void Save() => Document.Save(Filename);

        public bool Exists(string name)
        {
            XmlNodeList properties = Root.SelectNodes("property");

            if (properties?.Count > 0)
                foreach (XmlNode property in properties)
                    if (property.Attributes["id"]?.Value == name)
                        return true;

            return false;
        }

        private XmlAttribute GetProperty(string name)
        {
            if (!Exists(name))
                return null;

            XmlNodeList properties = Root.SelectNodes("property");

            foreach (XmlNode property in properties)
                if (property.Attributes["id"]?.Value == name)
                    return property.Attributes["value"];

            return null;
        }

        private XmlNode GetPropertyNode(string name)
        {
            XmlNodeList properties = Root.SelectNodes("property");

            if (properties?.Count > 0)
                foreach (XmlNode property in properties)
                    if (property.Attributes["id"]?.Value == name)
                        return property;

            return Document.CreateElement("property");
        }

        public bool GetBool(string name) => Convert.ToBoolean(GetPropertyNode(name).Attributes["value"]?.Value);

        public decimal GetDecimal(string name)
        {
            string temp = GetString(name);
            return temp == null ? 0 : Decimal.Parse(temp);
        }

        public string GetString(string name) => GetPropertyNode(name).Attributes["value"]?.Value;

        internal IEnumerable<T> GetObjects<T>(string name) => GetPropertyNode(name).ChildNodes.Cast<XmlNode>().Select(xn => xn.Attributes["value"]?.Value).Cast<T>();

        public IEnumerable<FolderBackupViewModel> GetPaths(string name)
        {
            return GetPropertyNode(name).ChildNodes.Cast<XmlNode>().Select(xn =>
            {
                var fb = FolderBackupViewModel.Parse(xn.Attributes["path"]?.Value);
                fb.Enabled = Boolean.Parse(xn.Attributes["enabled"]?.Value);
                return fb;
            }
            ).ToArray();
        }

        public void SetBool(string name, bool value)
        {
            XmlNode property = GetPropertyNode(name);
            XmlAttribute nameAttribute = Document.CreateAttribute("id");
            nameAttribute.Value = name;
            XmlAttribute valueAttribute = Document.CreateAttribute("value");
            valueAttribute.Value = value.ToString();

            property.Attributes.Append(nameAttribute);
            property.Attributes.Append(valueAttribute);

            Root.AppendChild(property);

            Save();
        }

        public void SetDecimal(string name, decimal value)
        {
            XmlNode property = GetPropertyNode(name);
            XmlAttribute nameAttribute = Document.CreateAttribute("id");
            nameAttribute.Value = name;
            XmlAttribute valueAttribute = Document.CreateAttribute("value");
            valueAttribute.Value = value.ToString();

            property.Attributes.Append(nameAttribute);
            property.Attributes.Append(valueAttribute);

            Root.AppendChild(property);

            Save();
        }

        public void SetPaths(string name, IEnumerable<FolderBackupViewModel> value)
        {
            XmlNode property = GetPropertyNode(name);
            property.RemoveAll();
            XmlAttribute nameAttribute = Document.CreateAttribute("id");
            nameAttribute.Value = name;
            foreach (var fb in value)
            {
                XmlNode element = Document.CreateElement("element");
                XmlAttribute valueAttribute = Document.CreateAttribute("path");
                valueAttribute.Value = fb.Path;
                element.Attributes.Append(valueAttribute);
                XmlAttribute enabledAttribute = Document.CreateAttribute("enabled");
                enabledAttribute.Value = fb.Enabled.ToString();
                element.Attributes.Append(enabledAttribute);
                property.AppendChild(element);
            }

            property.Attributes.Append(nameAttribute);

            Root.AppendChild(property);

            Save();
        }

        public void SetObjects<T>(string name, IEnumerable<T> values)
        {
            XmlNode property = GetPropertyNode(name);
            property.RemoveAll();
            XmlAttribute nameAttribute = Document.CreateAttribute("id");
            nameAttribute.Value = name;
            foreach (object obj in values)
            {
                XmlNode element = Document.CreateElement("element");
                XmlAttribute elementAttribute = Document.CreateAttribute("value");
                elementAttribute.Value = obj.ToString();
                element.Attributes.Append(elementAttribute);
                property.AppendChild(element);
            }

            property.Attributes.Append(nameAttribute);

            Root.AppendChild(property);

        }

        public void SetString(string name, string value)
        {
            XmlNode property = GetPropertyNode(name);
            XmlAttribute nameAttribute = Document.CreateAttribute("id");
            nameAttribute.Value = name;
            XmlAttribute valueAttribute = Document.CreateAttribute("value");
            valueAttribute.Value = value;

            property.Attributes.Append(nameAttribute);
            property.Attributes.Append(valueAttribute);

            Root.AppendChild(property);

            Save();
        }
    }
}
