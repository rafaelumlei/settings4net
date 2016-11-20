using settings4net.Core.Interfaces;
using System;
using System.Reflection;
using System.Xml;
using System.IO;
using log4net;

namespace settings4net.Core
{
    /// <summary>
    /// Class that extracts fields documentation from the assemblies' 
    /// XML (MSBuild generated) documentation file 
    /// </summary>
    internal class XMLDocumentationLoader : IDocumentationLoader
    {
        private static ILog logger = LogManager.GetLogger(typeof(XMLDocumentationLoader));

        private Assembly CurrentAssembly { get; set; }

        private XmlDocument AssemblyDocs { get; set; }

        public XMLDocumentationLoader(Assembly currentAssembly)
        {
            // Assembly.Location does not work because sometimes has a 
            // temp location that does not have the XML documents 
            this.CurrentAssembly = currentAssembly;
            string codeBase = currentAssembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            // extract this logic to the first access (to make it more lazy);
            string dllPath = Uri.UnescapeDataString(uri.Path);
            try
            {
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
            catch (Exception exp)
            {
                logger.Warn(string.Format("Expcetion when loading the xml documentation in {0}", dllPath), exp);
            }
        }

        public XMLDocumentationLoader(Type impType) : this(impType.Assembly)
        {
        }

        public string GetDocumentation(FieldInfo field)
        {
            if (this.AssemblyDocs != null)
            {
                XmlNamespaceManager manager = new XmlNamespaceManager(this.AssemblyDocs.NameTable);
                string fieldDocPath = "F:" + field.DeclaringType.FullName + "." + field.Name;
                XmlNode fieldDocNode = this.AssemblyDocs.SelectSingleNode("//member[starts-with(@name, '" + fieldDocPath + "')]", manager);
                
                return fieldDocNode?.InnerXml ?? string.Empty;
            }

            return string.Empty;
        }

    }
}
