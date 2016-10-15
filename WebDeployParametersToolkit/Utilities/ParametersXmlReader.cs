using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebDeployParametersToolkit.Utilities
{
    public static class ParametersXmlReader
    {
        public static IDictionary<string, string> GetParameters(string fileName)
        {
            var results = new Dictionary<string, string>(GetAutoParameters(fileName));

            var document = new XmlDocument();
            document.Load(fileName);
            var nav = document.CreateNavigator();
            nav.MoveToFirstChild();
            if (nav.Name != "parameters")
            {
                throw new FileFormatException($"Error parsing {fileName}. Expecting element parameters.");
            }
            nav.MoveToFirstChild();
            do
            {
                if (nav.NodeType == System.Xml.XPath.XPathNodeType.Element)
                {
                    if (nav.Name == "parameter")
                    {
                        var name = nav.GetAttribute("name", string.Empty);
                        var value = nav.GetAttribute("defaultvalue", string.Empty);
                        results.Add(name, value);
                    }
                    else if (nav.Name == "setParameter")
                    {                                               
                        var name = nav.GetAttribute("name", string.Empty);
                        var value = nav.GetAttribute("value", string.Empty);
                        if (!results.ContainsKey(name)) //might already be there via GetAutoParameters
                            results.Add(name, value);
                    }
                }
            } while (nav.MoveToNext());

            return results;
        }

        public static IDictionary<string, string> GetAutoParameters(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            var configFileName = Path.Combine(folder, "web.config");

            var results = GetConnectionStringParameters(configFileName);

            var project = VSPackage.DteInstance.Solution.FindProjectItem(fileName).ContainingProject;

            results.Add("IIS Web Application Name", project.Name);
            return results;
        }

        private static IDictionary<string, string> GetConnectionStringParameters(string configFileName)
        {
            var results = new Dictionary<string, string>();

            if (File.Exists(configFileName))
            {
                var document = new XmlDocument();

                document.Load(configFileName);
                var connectionStringsNode = document.SelectSingleNode("/configuration/connectionStrings");
                if (connectionStringsNode != null)
                {
                    var nav = connectionStringsNode.CreateNavigator();

                    if (nav.MoveToFirstChild())
                    {
                        do
                        {
                            if (nav.Name == "add")
                            {
                                results.Add($"{nav.GetAttribute("name", string.Empty)}-Web.config Connection String", nav.GetAttribute("connectionString", string.Empty));
                            }
                        } while (nav.MoveToNext());
                    }
                }
            }
            return results;
        }
    }
}