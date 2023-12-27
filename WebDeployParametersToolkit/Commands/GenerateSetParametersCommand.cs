﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Interop;
using System.Xml;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using WebDeployParametersToolkit.Extensions;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GenerateSetParametersCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b451bf5d-476a-43b7-8a00-11671601fdaa");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateSetParametersCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GenerateSetParametersCommand(AsyncPackage package, OleMenuCommandService commandService)
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
        public static GenerateSetParametersCommand Instance
        {
            get;
            private set;
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
            Instance = new GenerateSetParametersCommand(package, commandService);
        }

        private static bool CanGenerateSetParameters()
        {
            return !string.IsNullOrEmpty(SolutionExplorerExtensions.SelectedItemPath) && "Parameters.xml".Equals(Path.GetFileName(SolutionExplorerExtensions.SelectedItemPath), StringComparison.OrdinalIgnoreCase);
        }

        private void CreateSetXml(IEnumerable<WebDeployParameter> webDeployParameters, string fileName)
        {
            var writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true });

            try
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("parameters");
                foreach (var parameter in webDeployParameters)
                {
                    writer.WriteStartElement("setParameter");
                    writer.WriteAttributeString("name", parameter.Name);
                    writer.WriteAttributeString("value", parameter.DefaultValue);
                    writer.WriteEndElement();
                }

                writer.WriteEndDocument();
            }
            finally
            {
                writer.Close();
            }
        }

        private void GenerateFile(string fileName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var sourceName = SolutionExplorerExtensions.SelectedItemPath;
            var folder = Path.GetDirectoryName(sourceName);
            var targetName = Path.Combine(folder, fileName);

            if (File.Exists(targetName))
            {
                ShowMessage("Duplicate File", "The file name specified already exits.");
                return;
            }

            var projectFullName = VSPackage.DteInstance.Solution.FindProjectItem(sourceName).ContainingProject.FullName;

            var parameterizationProject = new ParameterizationProject(projectFullName);
            if (parameterizationProject.Initialize())
            {
                var projectName = VSPackage.DteInstance.Solution.FindProjectItem(sourceName).ContainingProject.Name;
                var parameters = ParseParameters(sourceName, projectName);
                CreateSetXml(parameters, targetName);
                var parent = VSPackage.DteInstance.Solution.FindProjectItem(sourceName);
                var item = parent.ProjectItems.AddFromFile(targetName);
                item.Properties.Item("ItemType").Value = "Parameterization";
                item.Open().Visible = true;
            }
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var menuItem = (OleMenuCommand)sender;
            menuItem.Visible = false;

            ThreadHelper.ThrowIfNotOnUIThread();
            SolutionExplorerExtensions.LoadSelectedItemPath();

            if (CanGenerateSetParameters())
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
            ThreadHelper.ThrowIfNotOnUIThread();
            var dialog = new FileNameDialog();
            var hwnd = VSPackage.DteInstance.MainWindow.HWnd;
            var window = (System.Windows.Window)HwndSource.FromHwnd(hwnd).RootVisual;
            dialog.Owner = window;

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                GenerateFile(dialog.Input);
            }
        }

        private IEnumerable<WebDeployParameter> ParseParameters(string fileName, string projectName)
        {
            IEnumerable<WebDeployParameter> results = null;
            try
            {
                var reader = new ParametersXmlReader(fileName, projectName);
                results = reader.Read();
            }
            catch (Exception ex)
            {
                ShowMessage("Parameters.xml Error", ex.Message);
            }

            return results;
        }

        private void ShowMessage(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
