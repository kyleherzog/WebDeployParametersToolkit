using System;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebDeployParametersToolkit
{
    public class ParameterizationProject
    {
        public ParameterizationProject(string fullName)
        {
            FullName = fullName;
        }

        public string FullName { get; }

        public bool NeedsInitialization
        {
            get
            {
                if (string.IsNullOrEmpty(FullName))
                {
                    return false;
                }

                using (var projectCollection = new Microsoft.Build.Evaluation.ProjectCollection())
                {
                    var buildProject = projectCollection.LoadProject(FullName);
                    var targets = buildProject.Xml.Targets;

                    return !targets.Any(t => t.Name == "SetParametersDeploy");
                }
            }
        }

        public bool Initialize()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (NeedsInitialization)
            {
                if (VSPackage.CoreDte.ItemOperations.PromptToSave == EnvDTE.vsPromptResult.vsPromptResultCancelled)
                {
                    return false;
                }
                else
                {
                    UnloadProject(FullName);
                    using (var projectCollection = new ProjectCollection())
                    {
                        var buildProject = projectCollection.LoadProject(FullName);
                        var targets = buildProject.Xml.Targets;

                        if (!targets.Any(t => t.Name == "SetParametersDeploy"))
                        {
                            var target = buildProject.Xml.CreateTargetElement("SetParametersDeploy");
                            buildProject.Xml.AppendChild(target);
                            target.AfterTargets = "Package";
                            target.AddTask("MakeDir").SetParameter("Directories", "$(PackageLocation)");
                            var copyTask = target.AddTask("Copy");
                            copyTask.SetParameter("SourceFiles", "@(Parameterization)");
                            copyTask.SetParameter("DestinationFolder", "$(PackageLocation)");

                            buildProject.Xml.Save();
                        }
                    }

                    ReloadProject(FullName);
                }
            }

            return true;
        }

        private static void ReloadProject(string projectFullName)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var solution = VSPackage.Solution as IVsSolution4;
            var projectGuid = GetProjectGuid(projectFullName);
            solution.ReloadProject(projectGuid);
        }

        private static void UnloadProject(string projectFullName)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var solution = VSPackage.Solution as IVsSolution4;
            var projectGuid = GetProjectGuid(projectFullName);

            solution.UnloadProject(projectGuid, (int)_VSProjectUnloadStatus.UNLOADSTATUS_UnloadedByUser);
        }

        private static Guid GetProjectGuid(string projectFullName)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var solution = VSPackage.Solution as IVsSolution;
            IVsHierarchy hierarchy;
            solution.GetProjectOfUniqueName(projectFullName, out hierarchy);

            Guid projectGuid;
            int hr;

            var itemIdRoot = 0xFFFFFFFE;

            hr = hierarchy.GetGuidProperty(itemIdRoot, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out projectGuid);
            ErrorHandler.ThrowOnFailure(hr);

            return projectGuid;
        }
    }
}
