using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;
using System.IO;

namespace settings4net.Core
{
    internal class XMLDocumentationLoader : IDocumentationLoader
    {

        private Assembly CurrentAssembly { get; set; }

        private XmlDocument AssemblyDocs { get; set; }

        public XMLDocumentationLoader(Assembly currentAssembly)
        {
            this.CurrentAssembly = currentAssembly;

            // extract this logic to the first access (to make it more lazy)
            try
            {
                string dllPath = this.CurrentAssembly.Location;

                if (!string.IsNullOrEmpty(dllPath))
                {
                    string xmlPath = Path.ChangeExtension(dllPath, ".XML");
                    if (File.Exists(xmlPath))
                    {
                        this.AssemblyDocs = new XmlDocument();
                        this.AssemblyDocs.Load(xmlPath);
                    }
                }
            }
            catch { }
        }

        public XMLDocumentationLoader(Type impType) : this(impType.Assembly)
        {
        }

        public string GetDocumentation(FieldInfo field)
        {
            if (this.AssemblyDocs != null)
            {
                string fieldDocPath = "F:" + field.DeclaringType.FullName + "." + field.Name;
                XmlNode fieldDocNode = this.AssemblyDocs.SelectSingleNode("//member[starts-with(@name, '" + fieldDocPath + "')]");
                return fieldDocNode?.InnerXml ?? string.Empty;
            }

            return string.Empty;
        }

    }
}
