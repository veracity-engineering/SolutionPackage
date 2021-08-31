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
		private List<KeyValuePair<string, string>> _subscriptions;
		private Dictionary<int, SecretsService> _secretsServices;
		private SecretsManagerStorage _storage;
		private DTE Dte;

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

			var configuration = SecretsManagerConfiguration.Load();
			if (configuration == null)
			{
				pnlSecrets.Visibility = Visibility.Collapsed;
				pnlConfiguration.Visibility = Visibility.Visible;
			}
			else
			{
				pnlConfiguration.Visibility = Visibility.Collapsed;
				pnlSecrets.Visibility = Visibility.Visible;
				IniailizeSecretsView(configuration);
			}
		}

		private void IniailizeSecretsView(SecretsManagerConfiguration configuration)
		{
			var variableGroupsConfig = new VariableGroupClientConfiguration
			{
				BaseUrl = configuration.VariableGroups.BaseUrl,
				Organization = configuration.VariableGroups.Organization,
				PersonalAccessToken = configuration.VariableGroups.PersonalAccessToken,
				ApiVersion = "6.1-preview.2"
			};
			_secretsServices = new Dictionary<int, SecretsService>
			{
				{ 0, new KeyVaultSecretsService() },
				{ 1, new VariableGroupSecretsService(variableGroupsConfig) }
			};
			cmbSourceTypes.Items.Clear();

			for (var index = 0; index < _sourceTypes.Count; index++)
			{
				cmbSourceTypes.Items.Add(_sourceTypes[index]);
			}
			_storage = SecretsManagerStorage.LoadOrCreate(_sourceTypes.ToArray());
			SetSourceTypeAsync(_storage.LastSourceTypeIndex);

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
			Dte.Events.WindowEvents.WindowActivated += OnWindowActivated;
		}

		private void OnWindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
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
			SetSourceTypeAsync(cmbSourceTypes.SelectedIndex);
		}

		private void btnDownload_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			SetFormBusy(true);
			var convertSecretsTask = DownloadSecretsAsync(cmbSourceTypes.SelectedIndex, _sources[cmbSources.SelectedIndex].Value);
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
			var convertSecretsTask = UploadSecretsAsync(cmbSourceTypes.SelectedIndex, _sources[cmbSources.SelectedIndex].Value, GetActiveDocumentText());
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
			cmbSourceSubscriptions.IsEnabled = value;
			cmbSources.IsEnabled = value;
			SetButtonAvailability(value);
		}

		private void SetButtonAvailability(bool value)
		{
			btnDownload.IsEnabled = _sources != null && cmbSources.SelectedIndex != -1 && _sources[cmbSources.SelectedIndex].Key != null
				? value
				: false;
			btnUpload.IsEnabled = value
				? IsActiveDocumentUploadable()
				: false;
		}

		private async Task SetSourceTypeAsync(int sourceTypeIndex)
		{
			if (sourceTypeIndex == -1)
			{
				cmbSourceSubscriptions.Items.Clear();
				cmbSources.Items.Clear();

				cmbSourceSubscriptions.Visibility = Visibility.Collapsed;
				cmbSources.Visibility = Visibility.Collapsed;
				return;
			}

			btnDownload.IsEnabled = false;
			btnUpload.IsEnabled = false;

			var sourceType = _storage.SourceTypes[sourceTypeIndex];

			if (sourceTypeIndex == 0)
			{
				cmbSources.Visibility = Visibility.Collapsed;

				cmbSourceSubscriptions.Text = "Working...";
				cmbSourceSubscriptions.IsEnabled = false;
				cmbSourceSubscriptions.Visibility = Visibility.Visible;

				var keyVaultSecretService = _secretsServices[0] as KeyVaultSecretsService;
				_subscriptions = (await keyVaultSecretService.GetSubscriptions()).ToList();
				cmbSourceSubscriptions.Items.Clear();
				foreach (var subscription in _subscriptions)
				{
					cmbSourceSubscriptions.Items.Add($"{subscription.Key}");
				}
				cmbSources.Items.Clear();
				cmbSourceSubscriptions.IsEnabled = true;
			}
			else
			{
				cmbSourceSubscriptions.Visibility = Visibility.Collapsed;

				cmbSources.Text = "Working...";
				cmbSources.IsEnabled = false;
				cmbSources.Visibility = Visibility.Visible;

				PopulateSources((await _secretsServices[sourceTypeIndex].GetSources()).OrderBy(s => s.Key).ToList());
			}

			cmbSourceTypes.SelectedIndex = sourceTypeIndex;
		}

		private void PopulateSources(List<KeyValuePair<string, string>> sources)
		{
			_sources = sources;
			cmbSources.Items.Clear();
			if (_sources != null)
			{
				foreach (var source in _sources)
				{
					cmbSources.Items.Add($"{source.Key} ({source.Value})");
				}
			}
			cmbSources.Text = string.Empty;
			cmbSources.IsEnabled = true;
			btnDownload.IsEnabled = false;
			btnUpload.IsEnabled = false;

			cmbSources.Visibility = Visibility.Visible;

			/*
			if (_sources != null)
			{
				var selectedSource = _sources.FirstOrDefault(s => s.Value.Equals(sourceType.Last, StringComparison.InvariantCultureIgnoreCase));
				if (selectedSource.Key != null)
					cmbSources.SelectedIndex = cmbSources.Items.IndexOf($"{selectedSource.Key} ({selectedSource.Value})");
			}
			*/
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

		private void btnConfigApply_Click(object sender, RoutedEventArgs e)
		{
			var configuration = new SecretsManagerConfiguration
			{
				VariableGroups = new VariableGroupsConfiguration
				{
					BaseUrl = txtBaseUrl.Text,
					Organization = txtBaseUrl.Text,
					PersonalAccessToken = txtPersonalAccessToken.Text
				}
			};
			configuration.Save();
			pnlConfiguration.Visibility = Visibility.Collapsed;
			pnlSecrets.Visibility = Visibility.Visible;
			IniailizeSecretsView(configuration);
		}

		private void cmbSourceSubscriptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			cmbSources.Items.Clear();
			cmbSources.Text = "Working...";
			cmbSources.IsEnabled = false;
			btnDownload.IsEnabled = false;
			btnUpload.IsEnabled = false;
			cmbSources.Visibility = Visibility.Visible;

			PopulateKeyVaultSourcesAsync(cmbSourceSubscriptions.SelectedIndex);
		}

		private async Task PopulateKeyVaultSourcesAsync(int subscriptionIndex)
		{
			var keyVaultSecretService = _secretsServices[0] as KeyVaultSecretsService;
			keyVaultSecretService.SetSubscriptionId(_subscriptions[subscriptionIndex].Value);
			var sources = await keyVaultSecretService.GetSources();
			PopulateSources(sources.OrderBy(s => s.Key).ToList());
		}

		private void cmbSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetButtonAvailability(cmbSources.SelectedIndex != -1);
		}
	}
}