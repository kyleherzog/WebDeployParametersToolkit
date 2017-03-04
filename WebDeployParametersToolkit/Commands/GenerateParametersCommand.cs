using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using WebDeployParametersToolkit.Extensions;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GenerateParametersCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 259;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b451bf5d-476a-43b7-8a00-11671601fdaa");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateParametersCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GenerateParametersCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus; ;
                commandService.AddCommand(menuItem);
            }
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var menuItem = (OleMenuCommand)sender;
            menuItem.Visible = false;

            SolutionExplorerExtensions.LoadSelectedItemPath();

            if (CanGenerateParameters())
            {
                menuItem.Visible = true;
            }
        }

        private bool CanGenerateParameters()
        {
            var filename = Path.GetFileName(SolutionExplorerExtensions.SelectedItemPath);
            return (filename.Equals("web.config", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GenerateParametersCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new GenerateParametersCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                var fileName = SolutionExplorerExtensions.SelectedItemPath;
                GenerateFile(fileName);
            }
            catch (Exception ex)
            {
                ShowMessage("Error Generating File", ex.Message);
            }
        }

        private void GenerateFile(string fileName)
        {
            var settings = GetWebConfigSettings(fileName);

            var folder = Path.GetDirectoryName(fileName);
            var targetName = Path.Combine(folder, "Parameters.xml");
            if (File.Exists(targetName))
            {
                if (VsShellUtilities.PromptYesNo("Merge missing settings into existing Parameters.xml file?",
                        "Update File?",
                        OLEMSGICON.OLEMSGICON_QUERY,
                        VSPackage.Shell))
                {
                    UpdateParametersXml(settings, targetName);
                }
            }
            else
            {
                CreateParametersXml(settings, targetName);
                var project = VSPackage.DteInstance.Solution.FindProjectItem(fileName).ContainingProject;
                project.ProjectItems.AddFromFile(targetName);
            }
            VSPackage.DteInstance.Solution.FindProjectItem(targetName).Open().Visible = true;
        }

        private IEnumerable<WebConfigSetting> GetWebConfigSettings(string fileName)
        {
            var reader = new WebConfigSettingsReader(fileName);

            reader.IncludeApplicationSettings = VSPackage.OptionsPage.IncludeApplicationSettings;
            reader.IncludeAppSettings = VSPackage.OptionsPage.IncludeAppSettings;
            reader.IncludeCompilationDebug = VSPackage.OptionsPage.IncludeCompilationDebug;
            reader.IncludeMailSettings = VSPackage.OptionsPage.IncludeMailSettings;
            reader.IncludeSessionStateSettings = VSPackage.OptionsPage.IncludeSessionStateSettings;

            return reader.Read();
        }

        private void UpdateParametersXml(IEnumerable<WebConfigSetting> settings, string fileName)
        {
            var parameters = ParametersXmlReader.GetParameters(fileName);
            var missingSettings = settings.Where(s => !parameters.Any(p => p.Key == s.Name)).ToList();

            var document = new XmlDocument();
            document.Load(fileName);

            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Fragment });
            try
            {
                WriteParameters(missingSettings, writer);
            }
            finally
            {
                writer.Close();
            }

            var missingParametersNodes = document.CreateDocumentFragment();
            missingParametersNodes.InnerXml = builder.ToString();

            var parametersNode = document.SelectSingleNode("/parameters");
            parametersNode.AppendChild(missingParametersNodes);

            document.Save(fileName);
        }

        private void CreateParametersXml(IEnumerable<WebConfigSetting> settings, string fileName)
        {
            XmlWriter writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true });

            try
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("parameters");
                WriteParameters(settings, writer);
                writer.WriteEndDocument();
            }
            finally
            {
                writer.Close();
            }
        }

        private void WriteParameters(IEnumerable<WebConfigSetting> settings, XmlWriter writer)
        {
            foreach (var setting in settings)
            {
                writer.WriteStartElement("parameter");
                writer.WriteAttributeString("name", setting.Name);
                writer.WriteAttributeString("defaultvalue", $"__{setting.Name.ToUpper(CultureInfo.CurrentCulture)}__");
                writer.WriteAttributeString("description", $"{setting.Name} description.");
                writer.WriteAttributeString("tags", string.Empty);

                writer.WriteStartElement("parameterentry");
                writer.WriteAttributeString("kind", "XmlFile");
                writer.WriteAttributeString("match", setting.NodePath);
                writer.WriteAttributeString("scope", @"\\web.config$");
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        private void ShowMessage(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                this.ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}