using System.IO;
using System.Xml;

namespace WebDeployParametersToolkit.Extensions
{
    public static class XmlDocumentExtensions
    {
        public static void SafeLoadXml(this XmlDocument document, string xml)
        {
            var stringReader = new StringReader(xml);
            var xmlReader = new XmlTextReader(stringReader) { DtdProcessing = DtdProcessing.Prohibit };
            document.Load(xmlReader);
        }
    }
}