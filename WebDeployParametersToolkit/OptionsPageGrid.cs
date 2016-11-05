using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDeployParametersToolkit
{
    public class OptionsPageGrid: DialogPage
    {
        [Category("Parameters to Generate")]
        [DisplayName("Mail Settings")]
        [Description("Generate parameters based on the /configuration/system.net/mailSettings node of the Web.Config")]
        public bool IncludeMailSettings { get; set; } = true;

        [Category("Parameters to Generate")]
        [DisplayName("Compilation Debug")]
        [Description("Generate parameter for the debug attribute of the /configuration/system.web/compilation node of the Web.Config")]
        public bool IncludeCompilationDebug { get; set; } = true;

        [Category("Parameters to Generate")]
        [DisplayName("Application Settings")]
        [Description("Generate parameters for the settings found under the /configuration/applicationSettings node of the Web.Config")]
        public bool IncludeApplicationSettings { get; set; } = true;

        [Category("Parameters to Generate")]
        [DisplayName("Session State Settings")]
        [Description("Generate parameters for the settings found under the /configuration/system.web/sessionState node of the Web.Config")]
        public bool IncludeSessionStateSettings { get; set; } = true;


    }
}
