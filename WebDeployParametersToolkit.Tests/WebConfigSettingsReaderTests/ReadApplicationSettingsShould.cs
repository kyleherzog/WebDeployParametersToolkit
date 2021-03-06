﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests.WebConfigSettingsReaderTests
{
    [TestClass]
    public class ReadApplicationSettingsShould
    {
        private const string standardSettingsRootPath = "/configuration/";
        private const string nestedSettingsRootPath = "/configuration/location[@path=\"subfolder\"]/";

        [TestMethod]
        public void ReturnAllTokenizedGivenTokenizeStyle()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleApplicationSettings(standardSettingsRootPath, style);
            var results = WebConfigSettingsReader.ReadApplicationSettings(simpleSettings.Document, true, true, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnAllWithValuesGivenCloneStyle()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleApplicationSettings(standardSettingsRootPath, style);
            var results = WebConfigSettingsReader.ReadApplicationSettings(simpleSettings.Document, true, true, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnTokenizedValuesGivenTokenizeStyleAndSettingsNestedUnderLocationTag()
        {
            var style = ParametersGenerationStyle.Tokenize;
            var simpleSettings = WebConfigSample.GetSimpleApplicationSettings(nestedSettingsRootPath, style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadApplicationSettings(locationSimpleSettings.Document, true, true, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnValuesGivenCloneStyleAndSettingsNestedUnderLocationTag()
        {
            var style = ParametersGenerationStyle.Clone;
            var simpleSettings = WebConfigSample.GetSimpleApplicationSettings(nestedSettingsRootPath, style);
            var locationSimpleSettings = WebConfigSample.GetLocationSimpleSettings();
            var results = WebConfigSettingsReader.ReadApplicationSettings(locationSimpleSettings.Document, true, true, style);
            simpleSettings.ExpectedSettings.AssertHasSameItems(results);
        }

        [TestMethod]
        public void ReturnZeroResultsGivenEmptySettings()
        {
            var emptySettings = WebConfigSample.GetEmptySettings();
            var results = WebConfigSettingsReader.ReadApplicationSettings(emptySettings.Document, true, true, ParametersGenerationStyle.Tokenize);
            emptySettings.ExpectedSettings.AssertHasSameItems(results);
        }
    }
}
