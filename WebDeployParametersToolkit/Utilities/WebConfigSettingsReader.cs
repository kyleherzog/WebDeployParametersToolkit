using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebDeployParametersToolkit.Utilities
{
    public class WebConfigSettingsReader
    {
        
        public WebConfigSettingsReader(string fileName)
        {
            FileName = fileName;
        }

        public bool IncludeApplicationSettings { get; set; } = true;

        public bool IncludeCompilationDebug { get; set; } = true;

        public bool IncludeMailSettings { get; set; } = true;

        public bool IncludeSessionStateSettings { get; set; } = true;

        public string FileName { get; }

        public IEnumerable<WebConfigSetting> Read()
        {
            var results = new List<WebConfigSetting>();

            if (File.Exists(FileName))
            {
                var document = new XmlDocument();
                document.Load(FileName);

                if (IncludeApplicationSettings)
                {
                    results.AddRange(ReadApplicationSettings(document));
                }
                if (IncludeApplicationSettings)
                {
                    results.Add(new WebConfigSetting() { Name = "CompilationDebug", NodePath = "/configuration/system.web/compilation/@debug" });
                }
                if (IncludeMailSettings)
                {
                    results.Add(new WebConfigSetting() { Name = "Smtp.NeworkHost", NodePath = "/configuration/system.net/mailSettings/smtp/network/@host" });
                    results.Add(new WebConfigSetting() { Name = "Smtp.DeliveryMethod", NodePath = "/configuration/system.net/mailSettings/smtp/@deliveryMethod" });
                }
                if (IncludeSessionStateSettings)
                {
                    results.Add(new WebConfigSetting() { Name = "SessionState.Mode", NodePath = "/configuration/system.web/sessionState/@mode" });
                    results.Add(new WebConfigSetting() { Name = "SessionState.ConnectionString", NodePath = "/configuration/system.web/sessionState/sqlConnectionString" });
                }

            }
            return results;
        }

        private static IEnumerable<WebConfigSetting> ReadApplicationSettings(XmlDocument document)
        {
            var results = new List<WebConfigSetting>();
            var basePath = "/configuration/applicationSettings";
            var settingsNode = document.SelectSingleNode(basePath);

            if (settingsNode != null)
            {
                var nav = settingsNode.CreateNavigator();
                if (nav.MoveToFirstChild())
                {
                    do
                    {
                        var groupName = nav.Name;
                        var groupPath = $"{basePath}/{nav.Name}";
                        if (nav.MoveToFirstChild())
                        {
                            do
                            {
                                var settingName = nav.GetAttribute("name", string.Empty);
                                var settingPath = $"{groupPath}/{nav.Name}[@name='{settingName}']/value/text()";

                                if (results.Exists(s => s.Name == settingName))
                                {
                                    settingName = $"{groupName}.{settingName}";
                                }

                                var setting = new WebConfigSetting()
                                {
                                    NodePath = settingPath,
                                    Name = settingName
                                };
                                results.Add(setting);
                            } while (nav.MoveToNext());
                        }
                        nav.MoveToParent();
                    } while (nav.MoveToNext());
                }
            }
            return results;
        }
    }
}