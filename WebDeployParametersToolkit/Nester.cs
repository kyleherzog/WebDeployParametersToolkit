using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;

namespace WebDeployParametersToolkit
{
    internal static class Nester
    {
        private static ProjectItemsEvents _events;

        public static void Initialize(DTE2 dte)
        {
            if (_events == null)
            {
                _events = ((Events2)dte.Events).ProjectItemsEvents;
                _events.ItemAdded += ItemAdded;
                _events.ItemRenamed += ItemRenamed;
            }
        }

        public static void ApplyNesting(string itemPath)
        {
            var item = VSPackage.DteInstance.Solution.FindProjectItem(itemPath);
            ApplyNesting(item);
        }

        private static void ItemRenamed(ProjectItem item, string oldName)
        {
            ItemAddedRenamed(item);
        }

        private static void ItemAdded(ProjectItem item)
        {
            ItemAddedRenamed(item);
        }

#pragma warning disable S1172 // Unused method parameters should be removed

        private static void ItemAddedRenamed(ProjectItem item)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            //TODO fix: this causes VS to throw a 'System.AccessViolationException' if it needs to initialize the project.
            //          Just commenting feature out for now.

#pragma warning disable S125 // Sections of code should not be "commented out"
            //if (item.ContainingProject != null)
            //{
            //    if (item.Properties != null && item.FileCount > 0)
            //    {
            //        var itemFileName = item.FileNames[0];
            //        var parameterizationProject = new ParameterizationProject(item.ContainingProject.FullName);
            //        if (parameterizationProject.Initialize())
            //        {
            //            //don't try to use the ProjectItem object.  It will fail if Initialization was done. Use the file
            //            ApplyNesting(itemFileName);
            //        }
            //    }
            //}
#pragma warning restore S125 // Sections of code should not be "commented out"
        }

        private static void ApplyNesting(ProjectItem item)
        {
            if (item == null)
            {
                return;
            }

            Guid kind;
            if (!Guid.TryParse(item.Kind, out kind))
            {
                return;
            }

            if (item.Properties != null && kind == VSConstants.ItemTypeGuid.PhysicalFile_guid)
            {
                var fullFileName = item.FileNames[0];
                var fileName = Path.GetFileName(fullFileName);
                var extension = Path.GetExtension(fileName);
                if (fileName.StartsWith("setparameters", StringComparison.OrdinalIgnoreCase) && extension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    var parentName = Path.Combine(Path.GetDirectoryName(fullFileName), "Parameters.xml");
                    var dte = VSPackage.DteInstance;
                    var parent = dte.Solution.FindProjectItem(parentName);
                    if (parent != null)
                    {
                        parent.ProjectItems.AddFromFile(fullFileName);
                    }
                }
            }
        }
    }
}
