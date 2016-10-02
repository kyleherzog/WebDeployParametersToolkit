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
                        results.Add(name, value);
                    }
                }
            } while (nav.MoveToNext());

            return results;
        }

        public static IDictionary<string, string> GetAutoParameters(string fileName)
        {
            var results = new Dictionary<string, string>();

            var project = VSPackage.DteInstance.Solution.FindProjectItem(fileName).ContainingProject;

            results.Add("IIS Web Application Name", project.Name);

            var folder = Path.GetDirectoryName(fileName);

            var document = new XmlDocument();
            document.Load(Path.Combine(folder, "web.config"));

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
            return results;
        }
    }
}