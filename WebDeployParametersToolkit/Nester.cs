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
            ApplyNesting(item);
        }

        private static void ItemAdded(ProjectItem item)
        {
            ApplyNesting(item);
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