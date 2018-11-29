using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests
{
    [TestClass]
    public class WebConfigSettingsReaderTests
    {
        [TestMethod]
        public void ReadApplicationSettingsWithEmptySettingsReturnsZeroResults()
        {
            var emptySettings = WebConfigSample.GetEmptySettings();
            var results = WebConfigSettingsReader.ReadApplicationSettings(emptySettings.Document, true, true, ParametersGenerationStyle.Tokenize);
            emptySettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadApplicationSettingsWithSimpleSettingsReturnsAllTokenized()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleApplicationSettings(style);
            var results = WebConfigSettingsReader.ReadApplicationSettings(simpleSettings.Document, true, true, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadApplicationSettingsWithSimpleSettingsReturnsAllWithValues()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleApplicationSettings(style);
            var results = WebConfigSettingsReader.ReadApplicationSettings(simpleSettings.Document, true, true, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadCompilationDebugSettingsWitEmpySettingsReturnsNull()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var emptySettings = WebConfigSample.GetEmptySettings();
            var result = WebConfigSettingsReader.ReadCompilationDebugSettings(emptySettings.Document, style);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReadCompilationDebugSettingsWithSimpleSettingsReturnsTokenized()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleCompilationDebugSettings(style);
            var results = WebConfigSettingsReader.ReadCompilationDebugSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadCompilationDebugSettingsWithSimpleSettingsReturnsWithValues()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleCompilationDebugSettings(style);
            var results = WebConfigSettingsReader.ReadCompilationDebugSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadMailSettingsWithEmptySettingsReturnsZeroResults()
        {
            var emptySettings = WebConfigSample.GetEmptySettings();
            var results = WebConfigSettingsReader.ReadMailSettings(emptySettings.Document, ParametersGenerationStyle.Tokenize);
            emptySettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadMailSettingsWithSimpleSettingsReturnsAllTokenized()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleMailSettings(style);
            var results = WebConfigSettingsReader.ReadMailSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadMailSettingsWithSimpleSettingsReturnsAllWithValues()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleMailSettings(style);
            var results = WebConfigSettingsReader.ReadMailSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadSessionStateSettingsWithEmptySettingsReturnsZeroResults()
        {
            var emptySettings = WebConfigSample.GetEmptySettings();
            var results = WebConfigSettingsReader.ReadSessionStateSettings(emptySettings.Document, ParametersGenerationStyle.Tokenize);
            emptySettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadSessionStateSettingsWithSimpleSettingsReturnsAllTokenized()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleSessionStateSettings(style);
            var results = WebConfigSettingsReader.ReadSessionStateSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadSessionStateSettingsWithSimpleSettingsReturnsAllWithValues()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleSessionStateSettings(style);
            var results = WebConfigSettingsReader.ReadSessionStateSettings(simpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadApplicationSettingsWithLocationSimpleSettingsReturnsAllTokenized()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleApplicationSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadApplicationSettings(locationSimpleSettings.Document, true, true, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadApplicationSettingsWithLocationSimpleSettingsReturnsAllWithValues()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleApplicationSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadApplicationSettings(locationSimpleSettings.Document, true, true, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadCompilationDebugSettingsWithLocationSimpleSettingsReturnsTokenized()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleCompilationDebugSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadCompilationDebugSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadCompilationDebugSettingsWithLocationSimpleSettingsReturnsWithValues()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleCompilationDebugSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadCompilationDebugSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadMailSettingsWithLocationSimpleSettingsReturnsAllTokenized()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleMailSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadMailSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadMailSettingsWithLocationSimpleSettingsReturnsAllWithValues()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleMailSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadMailSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadSessionStateSettingsWithLocationSimpleSettingsReturnsAllTokenized()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleSessionStateSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadSessionStateSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReadSessionStateSettingsWithLocationSimpleSettingsReturnsAllWithValues()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleSessionStateSettings(style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadSessionStateSettings(locationSimpleSettings.Document, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }
    }
}
