using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests
{
    internal class WebConfigSample
    {
        public WebConfigSample(string xml)
        {
            Document = new XmlDocument { XmlResolver = null };

            var sreader = new System.IO.StringReader(xml);
            using var reader = new XmlTextReader(sreader) { DtdProcessing = DtdProcessing.Prohibit };
            Document.Load(reader);

            ExpectedSettings = new List<WebConfigSetting>();
        }

        public XmlDocument Document { get; }

        public IList<WebConfigSetting> ExpectedSettings { get; }

        public static WebConfigSample GetEmptySettings()
        {
            var result = new WebConfigSample(Properties.Resources.EmptySettings);
            return result;
        }

        public static WebConfigSample GetLocationSimpleSettings()
        {
            var result = new WebConfigSample(Properties.Resources.LocationSimpleSettings);
            return result;
        }

        public static WebConfigSample GetSimpleApplicationSettings(string pathRoot, ParametersGenerationStyle style)
        {
            var result = new WebConfigSample(Properties.Resources.SimpleSettings);

            var appSettingsPathFormat = $"{pathRoot}appSettings/add[@key='{{0}}']/@value";
            result.AddExpectedApplicationSetting("AppSettingsKey", string.Format(CultureInfo.InvariantCulture, appSettingsPathFormat, "AppSettingsKey"), "0123", style);

            var applicationSettingsPathFormat = $"{pathRoot}applicationSettings/TestApp.Properties.Settings/setting[@name='{{0}}']/value/text()";
            result.AddExpectedApplicationSetting("SomeString", string.Format(CultureInfo.InvariantCulture, applicationSettingsPathFormat, "SomeString"), "String value is here.", style);
            result.AddExpectedApplicationSetting("SomeBoolean", string.Format(CultureInfo.InvariantCulture, applicationSettingsPathFormat, "SomeBoolean"), "True", style);

            return result;
        }

        public static WebConfigSample GetSimpleCompilationDebugSettings(ParametersGenerationStyle style)
        {
            var result = new WebConfigSample(Properties.Resources.SimpleSettings);

            result.AddExpectedApplicationSetting("Compilation.Debug", "/configuration/system.web/compilation/@debug", "true", style);
            return result;
        }

        public static WebConfigSample GetSimpleMailSettings(ParametersGenerationStyle style)
        {
            var result = new WebConfigSample(Properties.Resources.SimpleSettings);

            result.AddExpectedApplicationSetting("Smtp.NetworkHost", "/configuration/system.net/mailSettings/smtp/network/@host", "localhost", style);
            result.AddExpectedApplicationSetting("Smtp.DeliveryMethod", "/configuration/system.net/mailSettings/smtp/@deliveryMethod", "Network", style);

            return result;
        }

        public static WebConfigSample GetSimpleSessionStateSettings(ParametersGenerationStyle style)
        {
            var result = new WebConfigSample(Properties.Resources.SimpleSettings);

            result.AddExpectedApplicationSetting("SessionState.Mode", "/configuration/system.web/sessionState/@mode", "SQLServer", style);
            result.AddExpectedApplicationSetting("SessionState.ConnectionString", "/configuration/system.web/sessionState/@sqlConnectionString", "Data Source=myserver;Initial catalog=ASPState;User ID=aspsession;Password=passwordhere", style);

            return result;
        }

        public void AddExpectedApplicationSetting(string name, string nodePath, string value, ParametersGenerationStyle style)
        {
            var setting = new WebConfigSetting() { Name = name, NodePath = nodePath };
            if (style == ParametersGenerationStyle.Tokenize)
            {
                setting.Value = $"__{setting.Name.ToUpperInvariant()}__";
            }
            else
            {
                setting.Value = value;
            }

            ExpectedSettings.Add(setting);
        }
    }
}
