﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace WebDeployParametersToolkit.Extensions
{
    public static class SolutionExplorerExtensions
    {
        public static string SelectedItemPath { get; set; }

        public static void LoadSelectedItemPath()
        {
            var dte = VSPackage.DteInstance;

            ThreadHelper.ThrowIfNotOnUIThread();
            var paths = dte.ToolWindows.SolutionExplorer.SelectedItemPaths();

            if (paths.Count() == 1)
            {
                SelectedItemPath = paths.First();
            }
            else
            {
                SelectedItemPath = null;
            }
        }

        public static string NodeName(this UIHierarchyItem item)
        {
            var names = new List<string>();

            ThreadHelper.ThrowIfNotOnUIThread();

            AddAncestorNames(item, names);
            names.Reverse();
            return string.Join("\\", names);
        }

        public static IEnumerable<string> SelectedItemPaths(this UIHierarchy solutionExplorer)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var items = (Array)solutionExplorer.SelectedItems;

            foreach (UIHierarchyItem selItem in items)
            {
                if (selItem.Object is ProjectItem item && item.Properties != null)
                {
                    yield return item.Properties.Item("FullPath").Value.ToString();
                }
                else if (selItem.Object is Project project && project.Kind != ProjectKinds.vsProjectKindSolutionFolder)
                {
                    yield return project.RootFolderName();
                }
                else if (selItem.Object is Solution solution && !string.IsNullOrEmpty(solution.FullName))
                {
                    yield return Path.GetDirectoryName(solution.FullName);
                }
            }
        }

        private static void AddAncestorNames(UIHierarchyItem item, ICollection<string> names)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (item == null)
            {
                return;
            }

            names.Add(item.Name);
            if (item.Collection != null)
            {
                AddAncestorNames(item.Collection.Parent as UIHierarchyItem, names);
            }
        }
    }
}
