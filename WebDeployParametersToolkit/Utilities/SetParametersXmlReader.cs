using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebDeployParametersToolkit.Utilities
{
    public static class SetParametersXmlReader
    {
        public static IDictionary<string, string> GetParameters(string fileName)
        {
            var results = new Dictionary<string, string>();

            var document = new XmlDocument { XmlResolver = null };
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
                if (nav.NodeType == System.Xml.XPath.XPathNodeType.Element && nav.Name == "setParameter")
                {
                    var name = nav.GetAttribute("name", string.Empty);
                    var value = nav.GetAttribute("value", string.Empty);
                    results.Add(name, value);
                }
            }
            while (nav.MoveToNext());

            return results;
        }
    }
}
