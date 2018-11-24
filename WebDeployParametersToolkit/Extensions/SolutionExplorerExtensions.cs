using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace WebDeployParametersToolkit.Extensions
{
    public static class SolutionExplorerExtensions
    {
        public static string SelectedItemPath { get; set; }

        public static void LoadSelectedItemPath()
        {
            var dte = VSPackage.DteInstance;

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
            AddAncestorNames(item, names);
            names.Reverse();
            return string.Join("\\", names);
        }

        public static IEnumerable<string> SelectedItemPaths(this UIHierarchy solutionExplorer)
        {
            var items = (Array)solutionExplorer.SelectedItems;

            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as ProjectItem;
                var project = selItem.Object as Project;
                var solution = selItem.Object as Solution;

                if (item != null && item.Properties != null)
                {
                    yield return item.Properties.Item("FullPath").Value.ToString();
                }
                else if (project != null && project.Kind != ProjectKinds.vsProjectKindSolutionFolder)
                {
                    yield return project.RootFolderName();
                }
                else if (solution != null && !string.IsNullOrEmpty(solution.FullName))
                {
                    yield return Path.GetDirectoryName(solution.FullName);
                }
            }
        }

        private static void AddAncestorNames(UIHierarchyItem item, ICollection<string> names)
        {
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
