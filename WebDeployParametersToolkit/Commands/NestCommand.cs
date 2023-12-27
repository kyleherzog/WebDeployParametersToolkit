﻿//------------------------------------------------------------------------------
// <copyright file="NestCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using WebDeployParametersToolkit.Extensions;

namespace WebDeployParametersToolkit
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class NestCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 257;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b451bf5d-476a-43b7-8a00-11671601fdaa");

        /// <summary>
        /// Initializes a new instance of the <see cref="NestCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        private NestCommand(OleMenuCommandService commandService)
        {
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
        public static NestCommand Instance
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

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)).ConfigureAwait(false) as OleMenuCommandService;

            Instance = new NestCommand(commandService);
        }

        private static bool CanNestInParameters()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var itemPath = SolutionExplorerExtensions.SelectedItemPath;
            if (string.IsNullOrEmpty(itemPath))
            {
                return false;
            }

            var fileName = Path.GetFileName(itemPath);
            var extension = Path.GetExtension(itemPath);
            var directory = Path.GetDirectoryName(itemPath);
            var parametersItem = VSPackage.DteInstance.Solution.FindProjectItem(Path.Combine(directory, "Parameters.xml"));

            var setParametersItem = VSPackage.DteInstance.Solution.FindProjectItem(itemPath);

            var currentParent = setParametersItem?.Collection?.Parent as ProjectItem;

            return fileName.StartsWith("setparameters", StringComparison.OrdinalIgnoreCase)
                && extension.Equals(".xml", StringComparison.OrdinalIgnoreCase)
                && parametersItem != null
                && setParametersItem != null
                && (currentParent == null || currentParent.Name != parametersItem.Name);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var menuItem = (OleMenuCommand)sender;
            menuItem.Visible = false;

            SolutionExplorerExtensions.LoadSelectedItemPath();

            ThreadHelper.ThrowIfNotOnUIThread();

            if (CanNestInParameters())
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
            var dte = VSPackage.DteInstance;

            // save node name details to maintain selection in solution explorer
            var selectedItem = ((Array)dte.ToolWindows.SolutionExplorer.SelectedItems).Cast<UIHierarchyItem>().First();

            var selectedParent = selectedItem.Collection.Parent as UIHierarchyItem;
            var parentNodeName = selectedParent.NodeName();
            var nodeName = selectedItem.Name;
            var itemPath = SolutionExplorerExtensions.SelectedItemPath;

            var fileName = SolutionExplorerExtensions.SelectedItemPath;
            var projectFullName = VSPackage.DteInstance.Solution.FindProjectItem(fileName).ContainingProject.FullName;

            var parameterizationProject = new ParameterizationProject(projectFullName);
            if (parameterizationProject.Initialize())
            {
                var projectItem = VSPackage.DteInstance.Solution.FindProjectItem(fileName);
                projectItem.Properties.Item("ItemType").Value = "Parameterization";
                Nester.ApplyNesting(itemPath);

                System.Threading.Thread.Sleep(1000);
                dte.ToolWindows.SolutionExplorer.GetItem($"{parentNodeName}\\Parameters.xml").UIHierarchyItems.Expanded = true;
                dte.ToolWindows.SolutionExplorer.GetItem($"{parentNodeName}\\Parameters.xml\\{nodeName}").Select(vsUISelectionType.vsUISelectionTypeSelect);
            }
        }
    }
}