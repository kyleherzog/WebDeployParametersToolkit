using System;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebDeployParametersToolkit
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidWebDeployParametersToolkitPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(OptionsPageGrid), "Web Deploy Parameters Toolkit", "General", 0, 0, true)]
    public sealed class VSPackage : AsyncPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VSPackage"/> class.
        /// </summary>
        public VSPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        public static DTE CoreDte { get; set; }

        public static DTE2 DteInstance { get; set; }

        public static OptionsPageGrid OptionsPage { get; set; }

        public static IVsUIShell Shell { get; set; }

        public static SVsSolution Solution { get; set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">todo: describe cancellationToken parameter on InitializeAsync</param>
        /// <param name="progress">todo: describe progress parameter on InitializeAsync</param>
        /// <returns>Awaitable Task</returns>
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await GenerateSetParametersCommand.InitializeAsync(this).ConfigureAwait(true);
            await NestCommand.InitializeAsync(this);

            await base.InitializeAsync(cancellationToken, progress).ConfigureAwait(true);

            CoreDte = await GetServiceAsync(typeof(DTE)).ConfigureAwait(true) as DTE;
            DteInstance = await GetServiceAsync(typeof(DTE)).ConfigureAwait(true) as DTE2;
            Shell = await GetServiceAsync(typeof(SVsUIShell)).ConfigureAwait(true) as IVsUIShell;
            Solution = await GetServiceAsync(typeof(SVsSolution)).ConfigureAwait(true) as SVsSolution;

            OptionsPage = (OptionsPageGrid)GetDialogPage(typeof(OptionsPageGrid));

            await Nester.Initialize(DteInstance).ConfigureAwait(true);
            await ApplyMissingParametersCommand.InitializeAsync(this).ConfigureAwait(true);
            await GenerateParametersCommand.InitializeAsync(this).ConfigureAwait(true);
            await AddParameterizationTargetCommand.InitializeAsync(this).ConfigureAwait(true);
        }
    }
}
