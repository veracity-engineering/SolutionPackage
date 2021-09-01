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
			pnlSecrets.Visibility = Visibility.Collapsed;
			pnlConfiguration.Visibility = Visibility.Collapsed;
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
			SetFormState(new FormState
			{
				SourceTypes = FormComboState.Available,
				SubscriptionSources = FormComboState.Hidden,
				Sources = FormComboState.Hidden,
				ButtonsAvailable = false
			});

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

			AssignDteAsync().GetAwaiter().OnCompleted(() =>
			{
				cmbSourceTypes.SelectedIndex = _storage.LastSourceTypeIndex;
				//SetFormAvailable(true);
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
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Hidden,
					Sources = FormComboState.Hidden,
					ButtonsAvailable = false
				});
				return;
			}

			if (sourceTypeIndex == 0)
			{
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Busy,
					Sources = FormComboState.Hidden,
					ButtonsAvailable = false
				});

				var keyVaultSecretService = _secretsServices[0] as KeyVaultSecretsService;
				_subscriptions = (await keyVaultSecretService.GetSubscriptions()).ToList();
				cmbSourceSubscriptions.Items.Clear();
				foreach (var subscription in _subscriptions)
				{
					cmbSourceSubscriptions.Items.Add($"{subscription.Key}");
				}

				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Available,
					Sources = FormComboState.Busy,
					ButtonsAvailable = false
				});

				var lastSource = _storage.LastSources[0];
				if (lastSource != null)
					cmbSourceSubscriptions.SelectedIndex = _subscriptions.FindIndex(s => s.Value.Equals(lastSource[lastSource.First().Key]));
			}
			else
			{
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Hidden,
					Sources = FormComboState.Busy,
					ButtonsAvailable = false
				});

				var sources = await _secretsServices[sourceTypeIndex].GetSources();
				PopulateSources(sources.OrderBy(s => s.Key).ToList());
			}
		}

		private void PopulateSources(List<KeyValuePair<string, string>> sources)
		{
			_sources = sources;
			cmbSources.Items.Clear();
			if (_sources != null)
			{
				foreach (var source in _sources)
				{
					cmbSources.Items.Add($"{source.Key}");
				}
			}

			if (_sources != null)
			{
				var lastSource = _storage.LastSources[cmbSourceTypes.SelectedIndex];
				if (lastSource != null)
					cmbSources.SelectedIndex = _sources.FindIndex(s => s.Value.Equals(lastSource[lastSource.Last().Key]));
			}

			SetFormState(new FormState
			{
				SourceTypes = FormComboState.Available,
				SubscriptionSources = cmbSourceTypes.SelectedIndex == 0 ? FormComboState.Available : FormComboState.Hidden,
				Sources = FormComboState.Available,
				ButtonsAvailable = true
			});
		}

		private async Task DownloadSecretsAsync(int sourceTypeIndex, string source)
		{
			try
			{
				await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
				var result = await _secretsServices[sourceTypeIndex].GetSecretsAsJson(source);

				//_storage.AppendSource(sourceTypeIndex, source);
				if (sourceTypeIndex == 0)
				{
					var lastSource = new Dictionary<string, string>
					{
						{ _subscriptions[cmbSourceSubscriptions.SelectedIndex].Key, _subscriptions[cmbSourceSubscriptions.SelectedIndex].Value },
						{ _sources[cmbSources.SelectedIndex].Key, _sources[cmbSources.SelectedIndex].Value }
					};
					_storage.SetLast(sourceTypeIndex, lastSource);
				}
				if (sourceTypeIndex == 1)
				{
					var lastSource = new Dictionary<string, string>
					{
						{ _sources[cmbSources.SelectedIndex].Key, _sources[cmbSources.SelectedIndex].Value }
					};
					_storage.SetLast(sourceTypeIndex, lastSource);
				}
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

				//_storage.AppendSource(sourceTypeIndex, source);
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
					Organization = txtOrganization.Text,
					PersonalAccessToken = txtPersonalAccessToken.Text
				}
			};
			configuration.Save();
			pnlConfiguration.Visibility = Visibility.Collapsed;
			pnlSecrets.Visibility = Visibility.Visible;
			IniailizeSecretsView(configuration);
		}

		private void cmbSourceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var sourceTypeIndex = cmbSourceTypes.SelectedIndex;
			if (sourceTypeIndex == -1)
			{
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Hidden,
					Sources = FormComboState.Hidden,
					ButtonsAvailable = false
				});
			}
			if (sourceTypeIndex == 0)
			{
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Busy,
					Sources = FormComboState.Hidden,
					ButtonsAvailable = false
				});
			}
			if (sourceTypeIndex == 1)
			{
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Hidden,
					Sources = FormComboState.Busy,
					ButtonsAvailable = false
				});
			}
			SetSourceTypeAsync(cmbSourceTypes.SelectedIndex);
		}

		private void cmbSourceSubscriptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetFormState(new FormState
			{
				SourceTypes = FormComboState.Available,
				SubscriptionSources = FormComboState.Available,
				Sources = FormComboState.Busy,
				ButtonsAvailable = false
			});
			PopulateKeyVaultSourcesAsync(cmbSourceSubscriptions.SelectedIndex);
		}

		private void cmbSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetFormState(new FormState
			{
				SourceTypes = FormComboState.Available,
				SubscriptionSources = FormComboState.Available,
				Sources = FormComboState.Available,
				ButtonsAvailable = cmbSources.SelectedIndex != -1
			});
		}

		private async Task PopulateKeyVaultSourcesAsync(int subscriptionIndex)
		{
			var keyVaultSecretService = _secretsServices[0] as KeyVaultSecretsService;
			keyVaultSecretService.SetSubscriptionId(_subscriptions[subscriptionIndex].Value);
			var sources = await keyVaultSecretService.GetSources();
			PopulateSources(sources.OrderBy(s => s.Key).ToList());
		}

		private void SetFormState(FormState state)
		{
			ApplyFormComboState(cmbSourceTypes, state.SourceTypes);
			ApplyFormComboState(cmbSourceSubscriptions, state.SubscriptionSources);
			ApplyFormComboState(cmbSources, state.Sources);
			SetButtonAvailability(state.ButtonsAvailable);
		}

		private void ApplyFormComboState(ComboBox comboBox, FormComboState state)
		{
			if (state == FormComboState.Hidden)
			{
				comboBox.Visibility = Visibility.Collapsed;
				comboBox.IsEnabled = false;
			}
			if (state == FormComboState.Busy)
			{
				comboBox.Text = "Working...";
				comboBox.Visibility = Visibility.Visible;
				comboBox.IsEnabled = false;
			}
			if (state == FormComboState.Available)
			{
				comboBox.Visibility = Visibility.Visible;
				comboBox.IsEnabled = true;
			}
		}
	}

	internal struct FormState
	{
		public FormComboState SourceTypes { get; set; }
		public FormComboState SubscriptionSources { get; set; }
		public FormComboState Sources { get; set; }
		public bool ButtonsAvailable { get; set; }
	}

	internal enum FormComboState
	{
		Hidden,
		Available,
		Busy
	}
}