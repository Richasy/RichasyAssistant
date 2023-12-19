// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using Microsoft.Windows.AppLifecycle;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;
using Windows.Storage;
using Windows.System;

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
        ChatKernels = new ObservableCollection<ServiceMetadata>();
        DrawServices = new ObservableCollection<ServiceMetadata>();
        TranslateServices = new ObservableCollection<ServiceMetadata>();
        SpeechServices = new ObservableCollection<ServiceMetadata>();
        KernelExtraServices = new ObservableCollection<Items.SlimServiceItemViewModel>();
        StorageDisplayCountCollection = new ObservableCollection<int>()
        {
            50,
            100,
            200,
            500,
            1000,
            -1,
        };
        InternalKernel = new InternalKernelViewModel();
        InternalDrawService = new InternalDrawServiceViewModel();
        InternalTranslate = new InternalTranslateServiceViewModel();
        InternalSpeech = new InternalSpeechServiceViewModel();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        AppTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        UseMarkdownRenderer = SettingsToolkit.ReadLocalSetting(SettingNames.UseMarkdownRenderer, true);
        UseStreamOutput = SettingsToolkit.ReadLocalSetting(SettingNames.UseStreamOutput, true);
        AppVersion = AppToolkit.GetPackageVersion();
        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.CopyrightTemplate);
        Copyright = string.Format(copyrightTemplate, BuildYear);

        LibraryPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);

        StorageMaxDisplayCount = SettingsToolkit.ReadLocalSetting(SettingNames.MaxStorageSearchCount, 100);

        InitializeChatKernels();
        await InitializeDrawServicesAsync();
        await InitializeTranslateServicesAsync();
        await InitializeSpeechServicesAsync();
    }

    [RelayCommand]
    private async Task SaveAzureOpenAISettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAIAccessKey, InternalKernel.AzureOpenAIAccessKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAIEndpoint, InternalKernel.AzureOpenAIEndpoint);
        SettingsToolkit.WriteLocalSetting(SettingNames.DefaultAzureOpenAIChatModel, JsonSerializer.Serialize(InternalKernel.AzureOpenAIChatModel ?? new Metadata()));

        await AppViewModel.ResetSecretsAsync(
            SettingNames.AzureOpenAIAccessKey,
            SettingNames.AzureOpenAIEndpoint,
            SettingNames.DefaultAzureOpenAIChatModel);

        AppViewModel.ResetGlobalSettings();
    }

    [RelayCommand]
    private async Task SaveOpenAISettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIAccessKey, InternalKernel.OpenAIAccessKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAICustomEndpoint, InternalKernel.OpenAICustomEndpoint);
        SettingsToolkit.WriteLocalSetting(SettingNames.DefaultOpenAIChatModelName, InternalKernel.OpenAIChatModel?.Id ?? string.Empty);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIOrganization, InternalKernel.OpenAIOrganization);

        await AppViewModel.ResetSecretsAsync(
            SettingNames.OpenAIAccessKey,
            SettingNames.OpenAICustomEndpoint,
            SettingNames.OpenAIOrganization,
            SettingNames.DefaultAzureOpenAIChatModel);

        AppViewModel.ResetGlobalSettings();
    }

    [RelayCommand]
    private async Task SaveAzureDrawSettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureImageKey, InternalDrawService.AzureImageKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureImageEndpoint, InternalDrawService.AzureImageEndpoint);
        SettingsToolkit.WriteLocalSetting(SettingNames.DefaultAzureDrawModel, InternalDrawService.AzureDrawModel.Value);

        await AppViewModel.ResetSecretsAsync(
            SettingNames.AzureImageKey,
            SettingNames.AzureImageEndpoint,
            SettingNames.DefaultAzureDrawModel);

        AppViewModel.ResetGlobalSettings();
    }

    [RelayCommand]
    private async Task SaveOpenAIDrawSettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIImageKey, InternalDrawService.OpenAIImageKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAICustomEndpoint, InternalDrawService.OpenAICustomEndpoint);

        await AppViewModel.ResetSecretsAsync(
            SettingNames.OpenAIImageKey,
            SettingNames.OpenAICustomEndpoint);

        AppViewModel.ResetGlobalSettings();
    }

    [RelayCommand]
    private async Task SaveAzureTranslateSettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureTranslateKey, InternalTranslate.AzureTranslateKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureTranslateRegion, InternalTranslate.AzureTranslateRegion);

        await AppViewModel.ResetSecretsAsync(
            SettingNames.AzureTranslateKey,
            SettingNames.AzureTranslateRegion);

        AppViewModel.ResetGlobalSettings();
    }

    [RelayCommand]
    private async Task SaveBaiduTranslateSettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.BaiduTranslateAppId, InternalTranslate.BaiduTranslateAppId);
        SettingsToolkit.WriteLocalSetting(SettingNames.BaiduTranslateAppKey, InternalTranslate.BaiduTranslateKey);

        await AppViewModel.ResetSecretsAsync(
            SettingNames.BaiduTranslateAppId,
            SettingNames.BaiduTranslateAppKey);

        AppViewModel.ResetGlobalSettings();
    }

    [RelayCommand]
    private async Task SaveAzureSpeechSettingsAsync()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureSpeechKey, InternalSpeech.AzureSpeechKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureSpeechRegion, InternalSpeech.AzureSpeechRegion);

        await AppViewModel.ResetSecretsAsync(
            SettingNames.AzureSpeechKey,
            SettingNames.AzureSpeechRegion);

        AppViewModel.ResetGlobalSettings();
    }

    [RelayCommand]
    private async Task OpenLibraryAsync()
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(LibraryPath);
        await Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private void CloseLibrary()
    {
        LibraryPath = string.Empty;
        SettingsToolkit.DeleteLocalSetting(SettingNames.LibraryFolderPath);
        SettingsToolkit.DeleteLocalSetting(SettingNames.SkipWelcome);

        AppInstance.Restart(string.Empty);
    }

    [RelayCommand]
    private async Task ImportCustomKernelAsync()
    {
        var fileObj = await FileToolkit.PickFileAsync(".rapkg", AppViewModel.Instance.ActivatedWindow);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        try
        {
            await ExtractExtraServiceAsync(file.Path, ServiceType.Kernel);
            InitializeChatKernels();
            ExtraServiceViewModel.Instance.RefreshCommand.Execute(default);
        }
        catch (Exception ex)
        {
            AppViewModel.Instance.ShowTip(ex.Message, InfoType.Error);
            LogException(ex);
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

    partial void OnUseStreamOutputChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.UseStreamOutput, value);

    partial void OnStorageMaxDisplayCountChanged(int value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.MaxStorageSearchCount, value);
}
