using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests.WebConfigSettingsReaderTests
{
    [TestClass]
    public class ReadMailSettingsShould
    {
        [TestMethod]
        public void ReturnAllTokenizedGivenTokenizeStyle()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleMailSettings(style);
            var results = WebConfigSettingsReader.ReadMailSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnAllWithValuesGivenCloneStyle()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleMailSettings(style);
            var results = WebConfigSettingsReader.ReadMailSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnZeroResultsGivenEmptySettings()
        {
            var emptySettings = WebConfigSample.GetEmptySettings();
            var results = WebConfigSettingsReader.ReadMailSettings(emptySettings.Document, ParametersGenerationStyle.Tokenize);
            emptySettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnTokenizedValuesGivenTokenizeStyleAndSettingsNestedUnderLocationTag()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleMailSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadMailSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnValuesGivenCloneStyleAndSettingsNestedUnderLocationTag()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleMailSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadMailSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }
    }
}
