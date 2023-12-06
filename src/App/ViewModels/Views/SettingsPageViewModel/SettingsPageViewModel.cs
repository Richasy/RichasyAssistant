// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 设置页面的视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    public SettingsPageViewModel()
    {
        ChatKernels = new ObservableCollection<KernelMetadata>();
        InternalKernel = new InternalKernelViewModel();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        AppTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        UseMarkdownRenderer = SettingsToolkit.ReadLocalSetting(SettingNames.UseMarkdownRenderer, true);
        AppVersion = AppToolkit.GetPackageVersion();
        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.CopyrightTemplate);
        Copyright = string.Format(copyrightTemplate, BuildYear);

        LibraryPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
        await InitializeChatKernelsAsync();
    }

    [RelayCommand]
    private async Task SaveAzureOpenAISettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAIAccessKey, InternalKernel.AzureOpenAIAccessKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAIEndpoint, InternalKernel.AzureOpenAIEndpoint);
        SettingsToolkit.WriteLocalSetting(SettingNames.DefaultAzureOpenAIChatModelName, InternalKernel.AzureOpenAIChatModelName);

        await AppViewModel.ResetSecretsAsync(
            SettingNames.AzureOpenAIAccessKey,
            SettingNames.AzureOpenAIEndpoint,
            SettingNames.DefaultAzureOpenAIChatModelName);

        AppViewModel.ResetGlobalSettings();
    }

    [RelayCommand]
    private async Task SaveOpenAISettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIAccessKey, InternalKernel.OpenAIAccessKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAICustomEndpoint, InternalKernel.OpenAICustomEndpoint);
        SettingsToolkit.WriteLocalSetting(SettingNames.DefaultAzureOpenAIChatModelName, InternalKernel.OpenAIChatModelName);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIOrganization, InternalKernel.OpenAIOrganization);

        await AppViewModel.ResetSecretsAsync(
            SettingNames.OpenAIAccessKey,
            SettingNames.OpenAICustomEndpoint,
            SettingNames.OpenAIOrganization,
            SettingNames.DefaultAzureOpenAIChatModelName);

        AppViewModel.ResetGlobalSettings();
    }

    private async Task InitializeChatKernelsAsync()
    {
        TryClear(ChatKernels);
        ChatKernels.Add(new KernelMetadata("AzureOpenAI", "Azure Open AI"));
        ChatKernels.Add(new KernelMetadata("OpenAI", "Open AI"));

        var extraServicesPath = Path.Combine(LibraryPath, "_extraKernels.json");
        if (File.Exists(extraServicesPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(extraServicesPath);
                var data = JsonSerializer.Deserialize<List<KernelMetadata>>(json);
                foreach (var metadata in data)
                {
                    ChatKernels.Add(metadata);
                }
            }
            catch (Exception ex)
            {
                LogException(new Exception("Extra kernels load error", ex));
            }
        }

        var defaultKernel = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultKernel, KernelType.AzureOpenAI);
        if (defaultKernel == KernelType.AzureOpenAI)
        {
            ChatKernel = ChatKernels.First();
        }
        else if (defaultKernel == KernelType.OpenAI)
        {
            ChatKernel = ChatKernels[1];
        }
        else
        {
            var chatKernelId = SettingsToolkit.ReadLocalSetting(SettingNames.CustomKernelId, string.Empty);
            if (!string.IsNullOrEmpty(chatKernelId))
            {
                ChatKernel = ChatKernels.FirstOrDefault(p => p.Id == chatKernelId);
            }
        }

        if (ChatKernel == null)
        {
            AppViewModel.Instance.ShowTip(StringNames.ExtraKernelLoadFailed, InfoType.Warning);
        }
    }

    private void CheckTheme()
    {
        AppThemeText = AppTheme switch
        {
            ElementTheme.Light => ResourceToolkit.GetLocalizedString(StringNames.LightTheme),
            ElementTheme.Dark => ResourceToolkit.GetLocalizedString(StringNames.DarkTheme),
            _ => ResourceToolkit.GetLocalizedString(StringNames.SystemDefault),
        };
    }

    partial void OnAppThemeChanged(ElementTheme value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AppTheme, value);
        CheckTheme();
    }

    partial void OnUseMarkdownRendererChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.UseMarkdownRenderer, value);

    partial void OnChatKernelChanged(KernelMetadata value)
    {
        if (value == null)
        {
            return;
        }

        if (value.Id == "AzureOpenAI")
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultKernel, KernelType.AzureOpenAI);
        }
        else if (value.Id == "OpenAI")
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultKernel, KernelType.OpenAI);
        }
        else
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultKernel, KernelType.Custom);
            SettingsToolkit.WriteLocalSetting(SettingNames.CustomKernelId, value.Id);
        }

        AppViewModel.ResetGlobalSettings();
    }
}
