﻿using DNV.SecretsManager.Services;
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
		private Dictionary<int, Func<SecretsService>> _secretsServices;
		private SecretsManagerStorage _storage;
		private DTE Dte;

		private bool _isAllowUpload;

		private const int KeyVaultIndex = 0;
		private const int VariableGroupIndex = 1;

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
				btnClearCache.IsEnabled = _storage != null;
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
			};
			_isAllowUpload = configuration.IsAllowUpload;
			_secretsServices = new Dictionary<int, Func<SecretsService>>
			{
				{ KeyVaultIndex, () => new KeyVaultSecretsService() },
				{ VariableGroupIndex, () => new VariableGroupSecretsService(variableGroupsConfig) }
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
				SetSourceType();
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
			btnUpload.IsEnabled = _sources != null && cmbSources.SelectedIndex != -1 && IsActiveDocumentUploadable();
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

		private void btnClearCache_Click(object sender, RoutedEventArgs e)
		{
			_storage.Delete();
			btnClearCache.IsEnabled = false;
			if (_subscriptions != null)
				_subscriptions.Clear();
			if (_sources != null)
				_sourceTypes.Clear();
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
				},
				IsAllowUpload = chkAllowUpload.IsChecked.HasValue ? chkAllowUpload.IsChecked.Value : false
			};
			configuration.Save();
			pnlConfiguration.Visibility = Visibility.Collapsed;
			pnlSecrets.Visibility = Visibility.Visible;
			IniailizeSecretsView(configuration);
		}

		private void btnConfigCancel_Click(object sender, RoutedEventArgs e)
		{
			pnlConfiguration.Visibility = Visibility.Collapsed;
			pnlSecrets.Visibility = Visibility.Visible;
		}

		private void cmbSourceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetSourceType();
		}

		private void cmbSourceSubscriptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetSourceSubscription();
		}

		private void cmbSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetSource();
		}

		private void btnDownload_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			SetFormBusy(true);
			var convertSecretsTask = DownloadSecretsAsync(cmbSourceTypes.SelectedIndex, _sources[cmbSources.SelectedIndex].Value, _sources[cmbSources.SelectedIndex].Key);
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

		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			var configuration = SecretsManagerConfiguration.Load();
			txtBaseUrl.Text = configuration.VariableGroups.BaseUrl;
			txtOrganization.Text = configuration.VariableGroups.Organization;
			txtPersonalAccessToken.Text = configuration.VariableGroups.PersonalAccessToken;
			chkAllowUpload.IsChecked = configuration.IsAllowUpload;
			btnClearCache.IsEnabled = _storage != null;
			pnlSecrets.Visibility = Visibility.Collapsed;
			pnlConfiguration.Visibility = Visibility.Visible;
		}

		private async Task PopulateSubscriptionsAsync()
		{
			var fetchedFromCache = false;
			if (_storage != null && _storage.Sources != null && _storage.Sources.Any())
			{
				var sourceParents = _storage.Sources.Where(s => s.Parent != null).Select(s => s.Parent);
				if (sourceParents.Any())
				{
					_subscriptions = new List<KeyValuePair<string, string>>();
					foreach (var sourceParent in sourceParents)
					{
						_subscriptions.AddRange(sourceParent);
					}
					fetchedFromCache = true;
				}
			}
			if (!fetchedFromCache)
			{
				var keyVaultSecretService = _secretsServices[KeyVaultIndex]() as KeyVaultSecretsService;
				_subscriptions = (await keyVaultSecretService.GetSubscriptions()).ToList();

				_storage.Sources.AddRange(_subscriptions.Select(s => new SourceCache { Parent = new Dictionary<string, string> { { s.Key, s.Value } } }));
				_storage.Save();
			}
			cmbSourceSubscriptions.Items.Clear();
			foreach (var subscription in _subscriptions)
			{
				cmbSourceSubscriptions.Items.Add(subscription.Key);
			}

			var lastSource = _storage.GetLast(GetSourceCacheParent(KeyVaultIndex));
			cmbSourceSubscriptions.SelectedIndex = lastSource != null && lastSource.Any()
				? _subscriptions.FindIndex(s => s.Value.Equals(lastSource[lastSource.First().Key]))
				: -1;
			SetSourceSubscription();
		}

		private KeyValuePair<string, string>? GetSourceCacheParent(int sourceTypeIndex)
		{
			if (sourceTypeIndex == KeyVaultIndex)
				return _subscriptions[sourceTypeIndex];
			return null;
		}

		private async Task PopulateSourcesAsync()
		{
			var sourceTypeParent = GetSourceCacheParent(cmbSourceTypes.SelectedIndex);
			var fetchedFromCache = false;
			if (_storage != null && _storage.HasSourceCache(sourceTypeParent))
			{
				_sources = _storage.GetCachedSource(sourceTypeParent).ToList();
				fetchedFromCache = true;
			}
			if (!fetchedFromCache)
			{
				var secretService = _secretsServices[KeyVaultIndex]();
				if (cmbSourceTypes.SelectedIndex == KeyVaultIndex)
				{
					var keyVaultSecretService = secretService as KeyVaultSecretsService;
					keyVaultSecretService.SetSubscriptionId(_subscriptions[cmbSourceSubscriptions.SelectedIndex].Value);
				}
				_sources = (await secretService.GetSources()).ToList();
				_storage.AppendSourceCache(sourceTypeParent, _sources);
				_storage.Save();
			}

			cmbSources.Items.Clear();
			if (_sources != null)
			{
				foreach (var source in _sources.OrderBy(s => s.Key).ToList())
				{
					cmbSources.Items.Add(source.Key);
				}
			}

			var lastSource = _storage.GetLast(GetSourceCacheParent(cmbSourceTypes.SelectedIndex));
			cmbSources.SelectedIndex = lastSource != null && lastSource.Any()
				? _sources.FindIndex(s => s.Value.Equals(lastSource[lastSource.Last().Key]))
				: -1;
			SetSource();
		}

		private void SetSourceType()
		{
			if (cmbSourceTypes.SelectedIndex == -1)
			{
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Hidden,
					Sources = FormComboState.Hidden,
					ButtonsAvailable = false
				});
			}
			if (cmbSourceTypes.SelectedIndex == KeyVaultIndex)
			{
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Busy,
					Sources = FormComboState.Hidden,
					ButtonsAvailable = false
				});
				PopulateSubscriptionsAsync();
			}
			if (cmbSourceTypes.SelectedIndex == VariableGroupIndex)
			{
				SetFormState(new FormState
				{
					SourceTypes = FormComboState.Available,
					SubscriptionSources = FormComboState.Hidden,
					Sources = FormComboState.Busy,
					ButtonsAvailable = false
				});
				PopulateSourcesAsync();
			}
		}

		private void SetSourceSubscription()
		{
			SetFormState(new FormState
			{
				SourceTypes = FormComboState.Available,
				SubscriptionSources = FormComboState.Available,
				Sources = cmbSourceSubscriptions.SelectedIndex == -1
					? FormComboState.Hidden
					: FormComboState.Busy,
				ButtonsAvailable = false
			});
			PopulateSourcesAsync();
		}

		private void SetSource()
		{
			SetFormState(new FormState
			{
				SourceTypes = FormComboState.Available,
				SubscriptionSources = cmbSourceTypes.SelectedIndex == KeyVaultIndex
					? FormComboState.Available
					: FormComboState.Hidden,
				Sources = FormComboState.Available,
				ButtonsAvailable = cmbSources.SelectedIndex != -1
			});
		}

		private void SetFormState(FormState state)
		{
			ApplyFormComboState(cmbSourceTypes, state.SourceTypes);
			ApplyFormComboState(cmbSourceSubscriptions, state.SubscriptionSources);
			ApplyFormComboState(cmbSources, state.Sources);
			SetButtonAvailability(state.ButtonsAvailable);
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
				? _isAllowUpload && IsActiveDocumentUploadable()
				: false;
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
				comboBox.Text = comboBox.SelectedIndex == -1
					? string.Empty
					: comboBox.Items[comboBox.SelectedIndex].ToString();
				comboBox.Visibility = Visibility.Visible;
				comboBox.IsEnabled = true;
			}
		}

		private void SaveLastSource()
		{
			if (cmbSourceTypes.SelectedIndex == KeyVaultIndex)
			{
				var lastSource = new Dictionary<string, string>
				{
					{ _subscriptions[cmbSourceSubscriptions.SelectedIndex].Key, _subscriptions[cmbSourceSubscriptions.SelectedIndex].Value },
					{ _sources[cmbSources.SelectedIndex].Key, _sources[cmbSources.SelectedIndex].Value }
				};
				_storage.SetLast(cmbSourceTypes.SelectedIndex, GetSourceCacheParent(cmbSourceTypes.SelectedIndex), lastSource);
				_storage.Save();
			}
			if (cmbSourceTypes.SelectedIndex == VariableGroupIndex)
			{
				var lastSource = new Dictionary<string, string>
				{
					{ _sources[cmbSources.SelectedIndex].Key, _sources[cmbSources.SelectedIndex].Value }
				};
				_storage.SetLast(cmbSourceTypes.SelectedIndex, GetSourceCacheParent(cmbSourceTypes.SelectedIndex), lastSource);
				_storage.Save();
			}
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

		private async Task DownloadSecretsAsync(int sourceTypeIndex, string source, string sourceName)
		{
			try
			{
				await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
				var result = await _secretsServices[sourceTypeIndex]().GetSecretsAsJson(source);

				SaveLastSource();

				var path = $"{SecretsManagerStorage.StoragePath}/{sourceName}";
				var secretsFilename = $"{path}/{Guid.NewGuid():N}.json";

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
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
				await _secretsServices[sourceTypeIndex]().SetSecretsFromJson(source, content);

				SaveLastSource();

				MessageBox.Show($"Secrets uploaded successully to source '{source}'.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to upload secrets to source '{source}'.\n{ex.Message}");
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