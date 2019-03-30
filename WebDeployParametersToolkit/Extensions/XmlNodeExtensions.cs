using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebDeployParametersToolkit.Extensions
{
    public static class XmlNodeExtensions
    {
        public static string GetFullPath(this XmlNode node, IEnumerable<string> attributeIdentifiers = null)
        {
            if (attributeIdentifiers == null)
            {
                attributeIdentifiers = Enumerable.Empty<string>();
            }

            var nodeNames = new Stack<string>();
            var currentNode = node;
            do
            {
                var nodeName = new StringBuilder();
                nodeName.Append(currentNode.Name);
                foreach (var attributeIdentifier in attributeIdentifiers)
                {
                    var attribute = currentNode.Attributes.GetNamedItem(attributeIdentifier);
                    if (attribute != null)
                    {
                        nodeName.Append($"[@{attributeIdentifier}=\"{attribute.Value}\"]");
                    }
                }

                nodeNames.Push(nodeName.ToString());
                currentNode = currentNode.ParentNode;
            }
            while (currentNode.NodeType != XmlNodeType.Document);

            var result = new StringBuilder();
            foreach (var nodeName in nodeNames)
            {
                result.Append("/");
                result.Append(nodeName);
            }

            return result.ToString();
        }
    }
}
