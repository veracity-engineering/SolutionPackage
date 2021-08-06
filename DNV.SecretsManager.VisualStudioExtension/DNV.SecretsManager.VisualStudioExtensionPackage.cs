using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace DNV.SecretsManager.VisualStudioExtension
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(PackageGuidString)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideToolWindow(typeof(DNV.SecretsManager.VisualStudioExtension.SecretsManagerToolWindow))]
	public sealed class VisualStudioExtensionPackage : AsyncPackage
	{
		public const string PackageGuidString = "874819e4-79ac-4fc3-a774-7123573a7eb8";

		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
		    await SecretsManagerToolWindowCommand.InitializeAsync(this);
		}
	}
}
