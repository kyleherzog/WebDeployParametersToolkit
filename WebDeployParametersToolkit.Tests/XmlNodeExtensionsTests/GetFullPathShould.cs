using System;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Extensions;

namespace WebDeployParametersToolkit.Tests.XmlNodeExtensionsTests
{
    [TestClass]
    public class GetFullPathShould
    {
        private const string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><root><level1></level1><level1a path=\"folderNameHere\"><level2a></level2a></level1a></root>";

        [TestMethod]
        public void ReturnTheFullPathGivenNoAttributeIdentifiersSpecified()
        {
            var document = new XmlDocument() { XmlResolver = null };
            document.SafeLoadXml(xml);

            var node = document.SelectSingleNode("/root//level2a");
            var nodePath = node.GetFullPath();

            var expectedResult = "/root/level1a/level2a";
            Assert.AreEqual(expectedResult, nodePath);
        }

        [TestMethod]
        public void ReturnTheFullPathIncludingAttributeGivenAttributeIdentifierSpecified()
        {
            var document = new XmlDocument() { XmlResolver = null };
            document.SafeLoadXml(xml);

            var node = document.SelectSingleNode("/root//level2a");
            var nodePath = node.GetFullPath(new string[] { "path" });

            var expectedResult = "/root/level1a[@path=\"folderNameHere\"]/level2a";
            Assert.AreEqual(expectedResult, nodePath);
        }
    }
}
