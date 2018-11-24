using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace WebDeployParametersToolkit
{
    public enum ParametersGenerationStyle
    {
        Tokenize,
        Clone
    }

    public class OptionsPageGrid : DialogPage
    {
        [Category("Parameter Generation Style")]
        [DisplayName("Default Values")]
        [Description("Tokenize will generate default values in the format of __PARAMETERNAME__. Clone will just copy the current value from the config file.")]
        public ParametersGenerationStyle DefaultValueStyle { get; set; } = ParametersGenerationStyle.Clone;

        [Category("Parameters to Generate")]
        [DisplayName("Application Settings")]
        [Description("Generate parameters for the settings found under the /configuration/applicationSettings node of the Web.Config")]
        public bool IncludeApplicationSettings { get; set; } = true;

        [Category("Parameters to Generate")]
        [DisplayName("App Settings")]
        [Description("Generate parameters for the settings found under the /configuration/appSettings node of the Web.Config")]
        public bool IncludeAppSettings { get; set; } = true;

        [Category("Parameters to Generate")]
        [DisplayName("Compilation Debug")]
        [Description("Generate parameter for the debug attribute of the /configuration/system.web/compilation node of the Web.Config")]
        public bool IncludeCompilationDebug { get; set; } = true;

        [Category("Parameters to Generate")]
        [DisplayName("Mail Settings")]
        [Description("Generate parameters based on the /configuration/system.net/mailSettings node of the Web.Config")]
        public bool IncludeMailSettings { get; set; } = true;

        [Category("Parameters to Generate")]
        [DisplayName("Session State Settings")]
        [Description("Generate parameters for the settings found under the /configuration/system.web/sessionState node of the Web.Config")]
        public bool IncludeSessionStateSettings { get; set; } = true;
    }
}
