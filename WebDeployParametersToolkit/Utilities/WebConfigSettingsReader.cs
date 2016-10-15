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

        public bool SkipApplicationSettings { get; set; }

        public bool SkipCompilationDebug { get; set; }

        public string FileName { get; }

        public IEnumerable<WebConfigSetting> Read()
        {
            var results = new List<WebConfigSetting>();

            if (File.Exists(FileName))
            {
                var document = new XmlDocument();
                document.Load(FileName);

                if (!SkipApplicationSettings)
                {
                    results.AddRange(ReadApplicationSettings(document));
                }
                if (!SkipApplicationSettings)
                {
                    results.Add(new WebConfigSetting() { Name = "CompilationDebug", NodePath = "/configuration/system.web/compilation/@debug" });
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