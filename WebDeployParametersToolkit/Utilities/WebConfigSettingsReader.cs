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

        public ParametersGenerationStyle ValuesStyle { get; set; }

        public bool IncludeApplicationSettings { get; set; } = true;

        public bool IncludeAppSettings { get; set; } = true;

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

                results.AddRange(ReadApplicationSettings(document, IncludeAppSettings, IncludeApplicationSettings, ValuesStyle));
                if (IncludeCompilationDebug)
                {
                    var setting = ReadCompilationDebugSettings(document, ValuesStyle);
                    if (setting != null)
                    {
                        results.Add(setting);
                    }
                }
                if (IncludeMailSettings)
                {
                    results.AddRange(ReadMailSettings(document, ValuesStyle));
                }
                if (IncludeSessionStateSettings)
                {
                    results.AddRange(ReadSessionStateSettings(document, ValuesStyle));
                }

            }
            return results;
        }


        public static IEnumerable<WebConfigSetting> ReadSessionStateSettings(XmlDocument document, ParametersGenerationStyle style)
        {
            var results = new List<WebConfigSetting>();

            var sessionStatePath = "/configuration/system.web/sessionState";

            var modeValue = document.SelectSingleNode($"{sessionStatePath}")?.Attributes["mode"]?.Value;
            if (!string.IsNullOrEmpty(modeValue))
            {
                var setting = new WebConfigSetting() { Name = "SessionState.Mode", NodePath = $"{sessionStatePath}/@mode", Value = modeValue };
                if (style == ParametersGenerationStyle.Tokenize)
                {
                    setting.Value = TokenizeValue(setting.Name);
                }
                results.Add(setting);
            }

            var connectionStringValue = document.SelectSingleNode($"{sessionStatePath}")?.Attributes["sqlConnectionString"]?.Value;
            if (!string.IsNullOrEmpty(connectionStringValue))
            {
                var setting = new WebConfigSetting() { Name = "SessionState.ConnectionString", NodePath = $"{sessionStatePath}/@sqlConnectionString", Value = connectionStringValue };
                if (style == ParametersGenerationStyle.Tokenize)
                {
                    setting.Value = TokenizeValue(setting.Name);
                }
                results.Add(setting);
            }

            return results;
        }

        public static IEnumerable<WebConfigSetting> ReadMailSettings(XmlDocument document, ParametersGenerationStyle style)
        {
            var results = new List<WebConfigSetting>();

            var smtpPath = "/configuration/system.net/mailSettings/smtp";

            var hostValue = document.SelectSingleNode($"{smtpPath}/network")?.Attributes["host"]?.Value;
            if (!string.IsNullOrEmpty(hostValue))
            {
                var setting = new WebConfigSetting() { Name = "Smtp.NetworkHost", NodePath = $"{smtpPath}/network/@host", Value = hostValue };
                if (style == ParametersGenerationStyle.Tokenize)
                {
                    setting.Value = TokenizeValue(setting.Name);
                }
                results.Add(setting);
            }

            var deliveryMethodValue = document.SelectSingleNode($"{smtpPath}")?.Attributes["deliveryMethod"]?.Value;
            if (!string.IsNullOrEmpty(deliveryMethodValue))
            {
                var setting = new WebConfigSetting() { Name = "Smtp.DeliveryMethod", NodePath = $"{smtpPath}/@deliveryMethod", Value = deliveryMethodValue };
                if (style == ParametersGenerationStyle.Tokenize)
                {
                    setting.Value = TokenizeValue(setting.Name);
                }
                results.Add(setting);
            }

            return results;
        }

        public static WebConfigSetting ReadCompilationDebugSettings(XmlDocument document, ParametersGenerationStyle style)
        {
            var compilationNodePath = "/configuration/system.web/compilation";
            var result = new WebConfigSetting() { Name = "Compilation.Debug", NodePath = $"{compilationNodePath}/@debug" };
            result.Value = document.SelectSingleNode(compilationNodePath)?.Attributes["debug"]?.Value;

            if (string.IsNullOrEmpty(result.Value))
            {
                return null;
            }
            else
            {
                if (style == ParametersGenerationStyle.Tokenize)
                {
                    result.Value = TokenizeValue(result.Name);
                }

                return result;
            }
        }

        private static string TokenizeValue(string value)
        {
            return ($"__{value.ToUpperInvariant()}__");
        }

        public static IEnumerable<WebConfigSetting> ReadApplicationSettings(XmlDocument document, bool includeAppSettings, bool includeApplicationSettings, ParametersGenerationStyle style)
        {
            var results = new List<WebConfigSetting>();

            if (includeAppSettings)
            {
                var appSettingsPath = "/configuration/appSettings/add";
                var appSettingsNodes = document.SelectNodes(appSettingsPath);
                for (int i = 0; i < appSettingsNodes.Count; i++)
                {
                    var node = appSettingsNodes[i];
                    var keyAttribute = node.Attributes["key"];

                    if (keyAttribute != null)
                    {
                        var settingName = keyAttribute.Value;
                        var settingPath = $"{appSettingsPath}[@key='{settingName}']/@value";
                        var setting = (new WebConfigSetting()
                        {
                            Name = settingName,
                            NodePath = settingPath
                        });
                        if (style == ParametersGenerationStyle.Tokenize)
                        {
                            setting.Value = TokenizeValue(setting.Name);
                        }
                        else
                        {
                            setting.Value = node.Attributes["value"]?.Value;
                        }
                        results.Add(setting);
                    }
                }
            }

            if (includeApplicationSettings)
            {
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
                                    var serializeAs = nav.GetAttribute("serializeAs", string.Empty);
                                    if (serializeAs == "String")
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

                                        if (style == ParametersGenerationStyle.Tokenize)
                                        {
                                            setting.Value = TokenizeValue(setting.Name);
                                        }
                                        else
                                        {
                                            if (nav.MoveToFirstChild())
                                            {
                                                setting.Value = nav.Value;
                                                nav.MoveToParent();
                                            }
                                        }

                                        results.Add(setting);
                                    }
                                } while (nav.MoveToNext());
                            }
                            nav.MoveToParent();
                        } while (nav.MoveToNext());
                    }
                }
            }
            return results;
        }
    }
}