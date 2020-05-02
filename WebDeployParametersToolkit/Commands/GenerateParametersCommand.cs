using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
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
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateParametersCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GenerateParametersCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));

            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;

                commandService.AddCommand(menuItem);
            }
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
                return package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in SampleCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)).ConfigureAwait(true) as OleMenuCommandService;
            Instance = new GenerateParametersCommand(package, commandService);
        }

        private static bool CanGenerateParameters()
        {
            var filename = Path.GetFileName(SolutionExplorerExtensions.SelectedItemPath);
            return filename.Equals("web.config", StringComparison.OrdinalIgnoreCase);
        }

        private static void WriteParameters(IEnumerable<WebConfigSetting> settings, XmlWriter writer)
        {
            foreach (var setting in settings)
            {
                writer.WriteStartElement("parameter");
                writer.WriteAttributeString("name", setting.Name);
                writer.WriteAttributeString("defaultvalue", setting.Value);
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

        private void CreateParametersXml(IEnumerable<WebConfigSetting> settings, string fileName)
        {
            var writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true });

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

        private void EnsureUniqueSettingsNames(List<WebConfigSetting> settings, ICollection<string> usedNames)
        {
            foreach (var setting in settings)
            {
                if (usedNames.Any(n => n == setting.Name))
                {
                    setting.Name = GetUniqueName(setting.Name, usedNames, 2);
                }

                usedNames.Add(setting.Name);
            }
        }

        private async System.Threading.Tasks.Task GenerateFileAsync(string fileName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var settings = GetWebConfigSettings(fileName);

            var folder = Path.GetDirectoryName(fileName);
            var targetName = Path.Combine(folder, "Parameters.xml");
            if (File.Exists(targetName))
            {
                if (VsShellUtilities.PromptYesNo(
                        "Merge missing settings into existing Parameters.xml file?",
                        "Update File?",
                        OLEMSGICON.OLEMSGICON_QUERY,
                        VSPackage.Shell))
                {
                    await UpdateParametersXmlAsync(settings, targetName).ConfigureAwait(true);
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

        private string GetUniqueName(string baseName, IEnumerable<string> currentNames, int nextIndex)
        {
            if (currentNames.Any(n => n == $"{baseName}{nextIndex}"))
            {
                return GetUniqueName(baseName, currentNames, nextIndex + 1);
            }
            else
            {
                return $"{baseName}{nextIndex}";
            }
        }

        private IEnumerable<WebConfigSetting> GetWebConfigSettings(string fileName)
        {
            var reader = new WebConfigSettingsReader(fileName)
            {
                IncludeApplicationSettings = VSPackage.OptionsPage.IncludeApplicationSettings,
                IncludeAppSettings = VSPackage.OptionsPage.IncludeAppSettings,
                IncludeCompilationDebug = VSPackage.OptionsPage.IncludeCompilationDebug,
                IncludeMailSettings = VSPackage.OptionsPage.IncludeMailSettings,
                IncludeSessionStateSettings = VSPackage.OptionsPage.IncludeSessionStateSettings,
                ValuesStyle = VSPackage.OptionsPage.DefaultValueStyle
            };

            return reader.Read();
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
                ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    await GenerateFileAsync(fileName).ConfigureAwait(true);
                });
            }
            catch (Exception ex)
            {
                ShowMessage("Error Generating File", ex.Message);
            }
        }

        private void ShowMessage(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private async System.Threading.Tasks.Task UpdateParametersXmlAsync(IEnumerable<WebConfigSetting> settings, string fileName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var projectName = VSPackage.DteInstance.Solution.FindProjectItem(fileName).ContainingProject.Name;
            var reader = new ParametersXmlReader(fileName, projectName);
            var parameters = reader.Read();
            var matches = new List<string>();
            foreach (var parameter in parameters)
            {
                if (parameter.Entries != null)
                {
                    matches.AddRange(parameter.Entries.Select(e => e.Match));
                }
            }

            var missingSettings = settings.Where(s => !matches.Any(p => p == s.NodePath)).ToList();

            EnsureUniqueSettingsNames(missingSettings, parameters.Select(p => p.Name).ToList());

            var document = new XmlDocument { XmlResolver = null };
            var text = File.ReadAllText(fileName);
            var sreader = new StringReader(text);
            var xmlReader = new XmlTextReader(sreader) { DtdProcessing = DtdProcessing.Prohibit };
            document.Load(xmlReader);

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
    }
}
