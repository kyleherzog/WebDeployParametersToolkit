using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
