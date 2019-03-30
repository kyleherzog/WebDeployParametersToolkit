using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests.WebConfigSettingsReaderTests
{
    [TestClass]
    public class ReadCompilationDebugSettingsShould
    {
        [TestMethod]
        public void ReturnNullGivenEmptySettings()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var emptySettings = WebConfigSample.GetEmptySettings();
            var result = WebConfigSettingsReader.ReadCompilationDebugSettings(emptySettings.Document, style);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReturnTokenizedResultsGivenTokenizeStyle()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleCompilationDebugSettings(style);
            var results = WebConfigSettingsReader.ReadCompilationDebugSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnValuesGivenCloneStyle()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleCompilationDebugSettings(style);
            var results = WebConfigSettingsReader.ReadCompilationDebugSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnTokenizedValuesGivenTonenizeStyleAndSettingsNestedUnderLocationTag()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleCompilationDebugSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadCompilationDebugSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnValuesGivenCloneStyleAndSettingsNestedUnderLocationTag()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleCompilationDebugSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadCompilationDebugSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }
    }
}
