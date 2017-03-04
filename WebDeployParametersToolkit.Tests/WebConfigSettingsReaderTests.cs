using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests
{
    [TestClass]
    public class WebConfigSettingsReaderTests
    {
        [TestMethod]
        public void ReadApplicationSettingsWithSimpleSettingsReturnsAll()
        {
            var simpleSettings = WebConfigSample.GetSimpleSettings();
            var results = WebConfigSettingsReader.ReadApplicationSettings(simpleSettings.Document, true, true);
            Assert.IsTrue(simpleSettings.ExpectedApplicationSettings.HasSameItems(results));
        }

        [TestMethod]
        public void ReadApplicationSettingsWithEmptySettingsReturnsZeroResults()
        {
            var emptySettings = WebConfigSample.GetEmptySettings();
            var results = WebConfigSettingsReader.ReadApplicationSettings(emptySettings.Document, true, true);
            Assert.IsTrue(emptySettings.ExpectedApplicationSettings.HasSameItems(results));
        }
    }
}
