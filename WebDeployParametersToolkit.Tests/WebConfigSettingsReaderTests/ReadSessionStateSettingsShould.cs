using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests.WebConfigSettingsReaderTests
{
    [TestClass]
    public class ReadSessionStateSettingsShould
    {
        [TestMethod]
        public void ReturnTokenizedValuesGivenTokenizeStyle()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleSessionStateSettings(style);
            var results = WebConfigSettingsReader.ReadSessionStateSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnTokenizedValuesGivenTokenizeStyleAndSettingsNestedUnderLocationTag()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleSessionStateSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadSessionStateSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnValuesGivenCloneStyleAndSettingsNestedUnderLocatinoTag()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleSessionStateSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadSessionStateSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnZeroResultsGivenEmptySettings()
        {
            var emptySettings = WebConfigSample.GetEmptySettings();
            var results = WebConfigSettingsReader.ReadSessionStateSettings(emptySettings.Document, ParametersGenerationStyle.Tokenize);
            emptySettings.ExpectedSettings.AssertHasSameItems(results);
        }
    }
}
