using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Xml;
using WebDeployParametersToolkit.Extensions;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ApplyMissingParametersCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 258;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b451bf5d-476a-43b7-8a00-11671601fdaa");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyMissingParametersCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ApplyMissingParametersCommand(Package package)
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
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private ProjectItem ParametersXmlItem { get; set; }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var menuItem = (OleMenuCommand)sender;
            menuItem.Visible = false;

            SolutionExplorerExtensions.LoadSelectedItemPath();

            if (CanGenerateApplyMissingParameters())
            {
                menuItem.Visible = true;
            }
        }

        private bool CanGenerateApplyMissingParameters()
        {
            var item = SolutionExplorerExtensions.SelectedItemPath;
            if (string.IsNullOrEmpty(item))
                return false;

            var fileName = Path.GetFileName(item).ToLower();
            var extension = Path.GetExtension(item);
            var directory = Path.GetDirectoryName(item);
            ParametersXmlItem = WebDeployParametersToolkitPackage.DteInstance.Solution.FindProjectItem(Path.Combine(directory, "Parameters.xml"));

            return (fileName.StartsWith("setparameters") && extension == ".xml" && ParametersXmlItem != null);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ApplyMissingParametersCommand Instance
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
            Instance = new ApplyMissingParametersCommand(package);
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

                var allParameters = ParametersXmlReader.GetParameters(ParametersXmlItem.FileNames[0]);
                var existingParameters = ParametersXmlReader.GetParameters(fileName);

                var missingParameters = allParameters.Where(p => !existingParameters.Keys.Contains(p.Key)).ToList();

                if (missingParameters.Count == 0)
                {
                    ShowMessage("Parameters Already Updated", "No missing parameters found.");
                }

                var document = new XmlDocument();
                document.Load(SolutionExplorerExtensions.SelectedItemPath);

                var parametersNode = document.SelectSingleNode("/parameters");

                foreach (var parameter in missingParameters)
                {
                    var node = document.CreateElement("setParameter");
                    node.SetAttribute("name", parameter.Key);
                    node.SetAttribute("value", parameter.Value);
                    parametersNode.AppendChild(node);
                }

                document.Save(fileName);
            }
            catch (Exception ex)
            {
                ShowMessage("Parameters Error", ex.Message);
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