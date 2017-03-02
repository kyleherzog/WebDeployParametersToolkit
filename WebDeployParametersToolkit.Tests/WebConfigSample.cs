using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests
{
    class WebConfigSample
    {
        public WebConfigSample(string xml)
        {
            Document = new XmlDocument();
            Document.LoadXml(xml);
            ExpectedApplicationSettings = new List<WebConfigSetting>();
        }

        public IList<WebConfigSetting> ExpectedApplicationSettings { get; }

        public XmlDocument Document { get; }

        public void AddExpectedApplicationSetting(string name, string nodePath)
        {
            ExpectedApplicationSettings.Add(new WebConfigSetting() { Name = name, NodePath = nodePath });
        }


        public static WebConfigSample GetSimpleSettings()
        {
            var result = new WebConfigSample(Properties.Resources.SimpleSettings);

            var appSettingsPathFormat = "/configuration/appSettings/add[@key='{0}']/@value";
            result.AddExpectedApplicationSetting("AppSettingsKey", string.Format(appSettingsPathFormat, "AppSettingsKey"));

            var applicationSettinsPathFormat = "/configuration/applicationSettings/TestApp.Properties.Settings/setting[@name='{0}']/value/text()";
            result.AddExpectedApplicationSetting("SomeString", string.Format(applicationSettinsPathFormat, "SomeString"));
            result.AddExpectedApplicationSetting("SomeBoolean", string.Format(applicationSettinsPathFormat, "SomeBoolean"));

            return result;
        }

        public static WebConfigSample GetEmptySettings()
        {
            var result = new WebConfigSample(Properties.Resources.EmptySettings);
            return result;
        }
    }
}
