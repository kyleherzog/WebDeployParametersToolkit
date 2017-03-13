using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace WebDeployParametersToolkit.Utilities
{
    public class ParametersXmlReader
    {
        public ParametersXmlReader(string fileName, string projectName)
        {
            Document = new XmlDocument();
            Document.Load(fileName);

            ProjectName = projectName;

            var folder = Path.GetDirectoryName(fileName);
            var configFileName = Path.Combine(folder, "web.config");
            if (File.Exists(configFileName))
            {
                AutoParametersDocument = new XmlDocument();
                AutoParametersDocument.Load(configFileName);
            }
        }

        public ParametersXmlReader(XmlDocument document, string projectName): this(document, projectName, null)
        {
        }

        public ParametersXmlReader(string parametersXml, string projectName, string webConfigXml)
        {
            Document = new XmlDocument();
            Document.LoadXml(parametersXml);

            ProjectName = projectName;

            if (!string.IsNullOrEmpty(webConfigXml))
            {
                AutoParametersDocument = new XmlDocument();
                AutoParametersDocument.LoadXml(webConfigXml);
            }
        }

        public ParametersXmlReader(XmlDocument document, string projectName, XmlDocument autoParametersDocument)
        {
            Document = document;
            ProjectName = projectName;
            AutoParametersDocument = autoParametersDocument;
        }

        public XmlDocument Document { get; }

        public XmlDocument AutoParametersDocument { get; }

        public string ProjectName { get; }

        public IEnumerable<WebDeployParameter> Read()
        {
            var results = GetAutoParameters();

            var nav = Document.CreateNavigator();
            nav.MoveToFirstChild();
            if (nav.Name != "parameters")
            {
                throw new FileFormatException($"Error parsing parameters xml. Expecting element 'parameters'.");
            }
            if (nav.MoveToFirstChild())
            {
                do
                {
                    if (nav.NodeType == XPathNodeType.Element && nav.Name == "parameter")
                    {
                        var result = new WebDeployParameter()
                        {
                            Name = nav.GetAttribute("name", string.Empty),
                            DefaultValue = nav.GetAttribute("defaultvalue", string.Empty),
                            Description = nav.GetAttribute("description", string.Empty)
                        };
                        var entries = new List<WebDeployParameterEntry>();
                        if (nav.MoveToFirstChild())
                        {
                            do
                            {
                                if (nav.NodeType == XPathNodeType.Element && nav.Name == "parameterentry")
                                {
                                    entries.Add(new WebDeployParameterEntry()
                                    {
                                        Kind = nav.GetAttribute("kind", string.Empty),
                                        Match = nav.GetAttribute("match", string.Empty),
                                        Scope = nav.GetAttribute("scope", string.Empty)
                                    });
                                }
                            } while (nav.MoveToNext());
                        }
                        nav.MoveToParent();

                        result.Entries = entries;
                        results.Add(result);
                    }
                } while (nav.MoveToNext());
            }
            return results;
        }

        public ICollection<WebDeployParameter> GetAutoParameters()
        {
            var results = GetConnectionStringParameters();            

            results.Add(new WebDeployParameter()
            {
                Name = "IIS Web Application Name",
                DefaultValue = ProjectName
            });
            return results;
        }

        private ICollection<WebDeployParameter> GetConnectionStringParameters()
        {
            var results = new List<WebDeployParameter>();

            if (AutoParametersDocument != null)
            {
                var connectionStringsNode = AutoParametersDocument.SelectSingleNode("/configuration/connectionStrings");
                if (connectionStringsNode != null)
                {
                    var nav = connectionStringsNode.CreateNavigator();

                    if (nav.MoveToFirstChild())
                    {
                        do
                        {
                            if (nav.Name == "add")
                            {
                                var baseName = nav.GetAttribute("name", string.Empty);
                                var result = new WebDeployParameter()
                                {
                                    Name = $"{baseName}-Web.config Connection String",
                                    DefaultValue = nav.GetAttribute("connectionString", string.Empty),
                                    Description = $"{baseName} Connection String used in web.config by the application to access the database.",
                                    Entries = new List<WebDeployParameterEntry>()
                                    {
                                        new WebDeployParameterEntry()
                                        {
                                            Kind = "XmlFile",
                                            Match = $"/configuration/connectionStrings/add[@name='{baseName}']/@connectionString",
                                            Scope = @"\\web.config$" //This isn't exactly right it will be an exact path.
                                        }
                                    }
                                };

                                results.Add(result);
                            }
                        } while (nav.MoveToNext());
                    }
                }
            }
            return results;
        }
    }
}
