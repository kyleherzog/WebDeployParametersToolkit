using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using System;
using System.IO;

namespace WebDeployParametersToolkit
{
    internal class Nester
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

        private static void ItemRenamed(ProjectItem item, string oldName)
        {

            ItemAddedRenamed(item);
        }

        private static void ItemAdded(ProjectItem item)
        {
            ItemAddedRenamed(item);
        }

        private static void ItemAddedRenamed(ProjectItem item)
        {
            //TODO fix: this causes VS to throw a 'System.AccessViolationException' if it needs to initialize the project.
            //          Just commenting feature out for now.

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
        }

        public static void ApplyNesting(string itemPath)
        {
            var item = WebDeployParametersToolkitPackage.DteInstance.Solution.FindProjectItem(itemPath);
            ApplyNesting(item);
        }

        private static void ApplyNesting(ProjectItem item)
        {
            if (item == null)
                return;

            Guid kind;
            if (!Guid.TryParse(item.Kind, out kind))
                return;

            if (item.Properties != null && kind == VSConstants.ItemTypeGuid.PhysicalFile_guid)
            {
                var fullFileName = item.FileNames[0];
                var fileName = Path.GetFileName(fullFileName).ToLower();
                var extension = Path.GetExtension(fileName);
                if (fileName.StartsWith("setparameters") && extension == ".xml")
                {
                    var parentName = Path.Combine(Path.GetDirectoryName(fullFileName), "Parameters.xml");
                    var dte = WebDeployParametersToolkitPackage.DteInstance;
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