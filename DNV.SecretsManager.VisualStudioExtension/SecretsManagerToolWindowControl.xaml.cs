using DNV.SecretsManager.Services;
using DNV.SecretsManager.VisualStudioExtension.Storage;
using EnvDTE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DNV.SecretsManager.VisualStudioExtension
{
	public partial class SecretManagerToolWindowControl : UserControl
	{
		private List<string> _sourceTypes;
		private List<KeyValuePair<string, string>> _sources;
		private Dictionary<int, SecretsService> _secretsServices;
		private SecretsManagerStorage _storage;
		private DTE Dte;

		private static VariableGroupClientConfiguration _variableGroupClientConfiguration = new VariableGroupClientConfiguration
		{
			BaseUrl = "https://dnvgl-one.visualstudio.com",
			Organization = "Veracity",
			ApiVersion = "6.1-preview.2",
			PersonalAccessToken = "4gepm6cenmvsc3cox2hzymkddvof4dkx5xpwgi6x34tfcylyl6pa"
		};

		public SecretManagerToolWindowControl()
		{
			InitializeComponent();
		}

		private void SecretsManagerToolWindow_Loaded(object sender, RoutedEventArgs e)
		{
			SetFormAvailable(false);
			_sourceTypes = new List<string>
			{
				"Azure Key Vault",
				"Azure DevOps Variable Group"
			};
			_secretsServices = new Dictionary<int, SecretsService>
			{
				{ 0, new KeyVaultSecretsService() },
				{ 1, new VariableGroupSecretsService(_variableGroupClientConfiguration) }
			};
			cmbSourceTypes.Items.Clear();
			for (var index = 0; index < _sourceTypes.Count; index++)
			{
				cmbSourceTypes.Items.Add(_sourceTypes[index]);
			}
			_storage = SecretsManagerStorage.LoadOrCreate(_sourceTypes.ToArray());
			PopulateSources(_storage.LastSourceTypeIndex);

			AssignDteAsync().GetAwaiter().OnCompleted(() =>
			{
				SetFormAvailable(true);
			});
		}

		private async Task AssignDteAsync()
		{
			await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
			if (Dte == null)
				Dte = await Microsoft.VisualStudio.Shell.AsyncServiceProvider.GlobalProvider.GetServiceAsync(typeof(DTE)) as DTE;
			if (Dte == null)
				throw new ArgumentNullException(nameof(Dte));
			Dte.Events.WindowEvents.WindowActivated += OnWindowActivaed;
		}

		private void OnWindowActivaed(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
		{
			btnUpload.IsEnabled = IsActiveDocumentUploadable();
		}

		private bool IsActiveDocumentUploadable()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (Dte.ActiveDocument != null)
			{
				var text = GetActiveDocumentText();
				try
				{
					JsonConvert.DeserializeObject<object>(text);
					return true;
				}
				catch (Exception ex)
				{
					return false;
				}
			}
			return false;
		}

		private string GetActiveDocumentText()
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (Dte.ActiveDocument != null)
			{
				var activeDocument = Dte.ActiveDocument.Object("TextDocument") as TextDocument;
				var editPoint = activeDocument.CreateEditPoint();
				return editPoint.GetText(activeDocument.EndPoint);
			}
			return string.Empty;
		}

		private void cmbSourceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PopulateSources(cmbSourceTypes.SelectedIndex);
		}

		private void btnDownload_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			SetFormBusy(true);
			var convertSecretsTask = DownloadSecretsAsync(cmbSourceTypes.SelectedIndex, _sources[cmbSources.SelectedIndex].Value /*cmbSources.Text*/);
			convertSecretsTask.GetAwaiter()
				.OnCompleted(() =>
				{
					SetFormBusy(false);
				});
		}

		private void btnUpload_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			SetFormBusy(true);
			var convertSecretsTask = UploadSecretsAsync(cmbSourceTypes.SelectedIndex, _sources[cmbSources.SelectedIndex].Value /*cmbSources.Text*/, GetActiveDocumentText());
			convertSecretsTask.GetAwaiter()
				.OnCompleted(() =>
				{
					SetFormBusy(false);
				});
		}

		private void SetFormBusy(bool value)
		{
			SetFormAvailable(!value);
			lblWorking.Visibility = value
				? Visibility.Visible
				: Visibility.Hidden;
		}

		private void SetFormAvailable(bool value)
		{
			cmbSourceTypes.IsEnabled = value;
			cmbSources.IsEnabled = value;
			btnDownload.IsEnabled = value;
			btnUpload.IsEnabled = value
				? IsActiveDocumentUploadable()
				: false;
		}

		private async Task PopulateSources(int sourceTypeIndex)
		{
			if (sourceTypeIndex == -1)
			{
				cmbSources.Items.Clear();
				cmbSources.IsEnabled = false;
				btnDownload.IsEnabled = false;
				btnUpload.IsEnabled = false;
				return;
			}

			var sourceType = _storage.SourceTypes[sourceTypeIndex];
			_sources = (await _secretsServices[sourceTypeIndex].GetSources()).OrderBy(s => s.Key).ToList();

			cmbSourceTypes.SelectedIndex = sourceTypeIndex;
			cmbSources.Items.Clear();
			if (_sources != null)
			{
				foreach (var source in _sources)
				{
					cmbSources.Items.Add($"{source.Key} ({source.Value})");
				}
				//cmbSources.ItemsSource = sources.OrderBy(s => s.Key);
				//cmbSources.Items = sources.OrderBy(s => s.Key);
				//cmbSources.ITem
			}
			cmbSources.IsEnabled = true;
			btnDownload.IsEnabled = true;
			btnUpload.IsEnabled = false;
			//cmbSources.Text = sourceType.Last;
		}

		private async Task DownloadSecretsAsync(int sourceTypeIndex, string source)
		{
			try
			{
				await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
				var result = await _secretsServices[sourceTypeIndex].GetSecretsAsJson(source);

				_storage.AppendSource(sourceTypeIndex, source);
				_storage.Save();

				var secretsFilename = $"{SecretsManagerStorage.StoragePath}/secrets-{Guid.NewGuid():N}.json";
				File.WriteAllText(secretsFilename, result);

				Dte.ExecuteCommand("File.OpenFile", secretsFilename);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to download secrets from source '{source}'.\n{ex.Message}");
			}
		}

		private async Task UploadSecretsAsync(int sourceTypeIndex, string source, string content)
		{
			try
			{
				await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
				await _secretsServices[sourceTypeIndex].SetSecretsFromJson(source, content);

				_storage.AppendSource(sourceTypeIndex, source);
				_storage.Save();

				MessageBox.Show($"Secrets uploaded successully to source '{source}'.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to upload secrets to source '{source}'.\n{ex.Message}");
			}
		}
	}
}