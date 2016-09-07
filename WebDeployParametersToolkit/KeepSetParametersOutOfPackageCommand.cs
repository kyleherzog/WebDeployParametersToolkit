//------------------------------------------------------------------------------
// <copyright file="KeepSetParametersOutOfPackageCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using WebDeployParametersToolkit.Extensions;
using WebDeployParametersToolkit.Utilities;
using System.IO;
using EnvDTE;
using System.Diagnostics;

namespace WebDeployParametersToolkit
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class KeepSetParametersOutOfPackageCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 260;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b451bf5d-476a-43b7-8a00-11671601fdaa");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeepSetParametersOutOfPackageCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private KeepSetParametersOutOfPackageCommand(Package package)
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

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var menuItem = (OleMenuCommand)sender;
            menuItem.Visible = false;

            SolutionExplorerExtensions.LoadSelectedItemPath();

            if (CanApplySetParametersBuildOptions())
            {
                menuItem.Visible = true;
            }
        }

        private bool CanApplySetParametersBuildOptions()
        {
            return (!string.IsNullOrEmpty(SolutionExplorerExtensions.SelectedItemPath) && string.Compare("Parameters.xml", Path.GetFileName(SolutionExplorerExtensions.SelectedItemPath), true) == 0);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static KeepSetParametersOutOfPackageCommand Instance
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
            Instance = new KeepSetParametersOutOfPackageCommand(package);
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
            var fileName = SolutionExplorerExtensions.SelectedItemPath;
            var project = WebDeployParametersToolkitPackage.DteInstance.Solution.FindProjectItem(fileName).ContainingProject;
            var properties = project.Properties;
            foreach (Property property in properties)
            {
 Debug.WriteLine(property.Name);
            }
           
            //string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            //string title = "KeepSetParametersOutOfPackageCommand";

            //// Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.ServiceProvider,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
