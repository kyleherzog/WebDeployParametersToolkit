using System.Collections.Generic;
using System.Xml;

namespace WebDeployParametersToolkit.Utilities
{
    public static class WebConfigSettingsReader
    {
        public static IEnumerable<WebConfigSetting> ReadSettings(string fileName)
        {
            var results = new List<WebConfigSetting>();

            var document = new XmlDocument();
            document.Load(fileName);

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