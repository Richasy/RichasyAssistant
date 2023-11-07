// Copyright (c) Richasy Assistant. All rights reserved.

using System.Diagnostics;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Locator;
using Windows.Storage;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 欢迎页面视图模型.
/// </summary>
public sealed partial class WelcomePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomePageViewModel"/> class.
    /// </summary>
    private WelcomePageViewModel()
    {
        StepCount = 7;
        CurrentStep = 0;
        CheckStep();
        CheckKernelType();
        CheckTranslateType();
        CheckSpeechType();
        CheckImageType();

        AzureOpenAIChatModelCollection = new ObservableCollection<string>();
        AzureOpenAICompletionModelCollection = new ObservableCollection<string>();
        AzureOpenAIEmbeddingModelCollection = new ObservableCollection<string>();
        OpenAIChatModelCollection = new ObservableCollection<string>();
        OpenAICompletionModelCollection = new ObservableCollection<string>();
        OpenAIEmbeddingModelCollection = new ObservableCollection<string>();
    }

    [RelayCommand]
    private async Task RestartAsync()
    {
        if (CurrentStep >= 2)
        {
            WriteSettings();
            await AppViewModel.ResetSecretsAsync();
        }

        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
        if (string.IsNullOrEmpty(libPath))
        {
            AppViewModel.Instance.ShowTip(StringNames.LibraryNeedInitialized, InfoType.Error);
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.SkipWelcome, true);
        Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().UnregisterKey();
        Application.Current.Exit();
        var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c start ricass://";
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
    }

    [RelayCommand]
    private async Task GoNextAsync()
    {
        if (CurrentStep < StepCount - 1)
        {
            CurrentStep++;
        }
        else
        {
            await RestartAsync();
        }
    }

    [RelayCommand]
    private async Task CreateLibraryAsync()
    {
        var folderObj = await FileToolkit.PickFolderAsync(AppViewModel.Instance.ActivatedWindow);
        if (folderObj is StorageFolder folder)
        {
            var hasFiles = Directory.GetFiles(folder.Path).Length > 0;
            if (hasFiles)
            {
                AppViewModel.Instance.ShowTip(StringNames.FolderMustEmpty, InfoType.Error);
                return;
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.LibraryFolderPath, folder.Path);
            await GoNextAsync();
        }
    }

    [RelayCommand]
    private async Task OpenLibraryAsync()
    {
        var folderObj = await FileToolkit.PickFolderAsync(AppViewModel.Instance.ActivatedWindow);
        if (folderObj is StorageFolder folder)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LibraryFolderPath, folder.Path);
            var secretFilePath = Path.Combine(folder.Path, "_secret_.db");
            if (!File.Exists(secretFilePath))
            {
                // 没有可用的配置文件，仍需要完成剩余配置.
                await GoNextAsync();
            }
            else
            {
                await AppViewModel.RetrieveSecretsAsync();
                await RestartAsync();
            }
        }
    }

    [RelayCommand]
    private async Task TryLoadAIModelSourceAsync()
    {
        if ((IsAzureOpenAI && AzureOpenAIChatModelCollection.Count > 0)
            || (IsOpenAI && OpenAIChatModelCollection.Count > 0)
            || (IsAzureOpenAI && (string.IsNullOrEmpty(AzureOpenAIAccessKey) || string.IsNullOrEmpty(AzureOpenAIEndpoint)))
            || (IsOpenAI && string.IsNullOrEmpty(OpenAIAccessKey)))
        {
            return;
        }

        try
        {
            if (IsAzureOpenAI)
            {
                GlobalSettings.Set(SettingNames.AzureOpenAIAccessKey, AzureOpenAIAccessKey);
                GlobalSettings.Set(SettingNames.AzureOpenAIEndpoint, AzureOpenAIEndpoint);
                var (chatModels, textCompletions, embeddings) = await ChatClient.GetSupportModelsAsync(KernelType.AzureOpenAI);
                TryClear(AzureOpenAIChatModelCollection);
                TryClear(AzureOpenAICompletionModelCollection);
                TryClear(AzureOpenAIEmbeddingModelCollection);
                foreach (var item in chatModels)
                {
                    AzureOpenAIChatModelCollection.Add(item);
                }

                foreach (var item in textCompletions)
                {
                    AzureOpenAICompletionModelCollection.Add(item);
                }

                foreach (var item in embeddings)
                {
                    AzureOpenAIEmbeddingModelCollection.Add(item);
                }
            }
            else
            {
                GlobalSettings.Set(SettingNames.OpenAIAccessKey, AzureOpenAIAccessKey);
                var (chatModels, textCompletions, embeddings) = await ChatClient.GetSupportModelsAsync(KernelType.OpenAI);
                TryClear(OpenAIChatModelCollection);
                TryClear(OpenAICompletionModelCollection);
                TryClear(OpenAIEmbeddingModelCollection);
                foreach (var item in chatModels)
                {
                    OpenAIChatModelCollection.Add(item);
                }

                foreach (var item in textCompletions)
                {
                    OpenAICompletionModelCollection.Add(item);
                }

                foreach (var item in embeddings)
                {
                    OpenAIEmbeddingModelCollection.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Debug(ex.Message);
        }
    }

    [RelayCommand]
    private async Task TryLoadWhisperModelAsync()
    {
        if (!IsAzureWhisper
            || string.IsNullOrEmpty(AzureWhisperKey)
            || string.IsNullOrEmpty(AzureWhisperEndpoint))
        {
            return;
        }

        try
        {
            GlobalSettings.Set(SettingNames.AzureWhisperKey, AzureWhisperKey);
            GlobalSettings.Set(SettingNames.AzureWhisperEndpoint, AzureWhisperEndpoint);

            // TODO: 加载模型.
            await Task.Delay(200);
            AzureWhisperModelName = "whisper";
        }
        catch (Exception ex)
        {
            Logger.Debug(ex.Message);
        }
    }

    private void WriteSettings()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAIAccessKey, AzureOpenAIAccessKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAIEndpoint, AzureOpenAIEndpoint);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAIChatModelName, AzureOpenAIChatModelName);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAICompletionModelName, AzureOpenAICompletionModelName);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureOpenAIEmbeddingModelName, AzureOpenAIEmbeddingModelName);

        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIAccessKey, OpenAIAccessKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAICustomEndpoint, OpenAICustomEndpoint);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIOrganization, OpenAIOrganization);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIChatModelName, OpenAIChatModelName);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAICompletionModelName, OpenAICompletionModelName);
        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIEmbeddingModelName, OpenAIEmbeddingModelName);

        SettingsToolkit.WriteLocalSetting(SettingNames.AzureTranslateKey, AzureTranslateKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureTranslateRegion, AzureTranslateRegion);

        SettingsToolkit.WriteLocalSetting(SettingNames.BaiduTranslateAppId, BaiduTranslateAppId);
        SettingsToolkit.WriteLocalSetting(SettingNames.BaiduTranslateAppKey, BaiduTranslateKey);

        SettingsToolkit.WriteLocalSetting(SettingNames.AzureSpeechKey, AzureSpeechKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureSpeechRegion, AzureSpeechRegion);

        SettingsToolkit.WriteLocalSetting(SettingNames.AzureWhisperKey, AzureWhisperKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureWhisperEndpoint, AzureWhisperEndpoint);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureWhisperModelName, AzureWhisperModelName);

        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIWhisperKey, OpenAIWhisperKey);

        SettingsToolkit.WriteLocalSetting(SettingNames.AzureImageKey, AzureImageKey);
        SettingsToolkit.WriteLocalSetting(SettingNames.AzureImageEndpoint, AzureImageEndpoint);

        SettingsToolkit.WriteLocalSetting(SettingNames.OpenAIImageKey, OpenAIImageKey);
    }

    private void CheckStep()
    {
        IsFREStep = CurrentStep == 0;
        IsLibraryStep = CurrentStep == 1;
        IsAIStep = CurrentStep == 2;
        IsTranslateStep = CurrentStep == 3;
        IsSpeechStep = CurrentStep == 4;
        IsImageStep = CurrentStep == 5;
        IsLastStep = CurrentStep == StepCount - 1;
    }

    private void CheckKernelType()
    {
        IsAzureOpenAI = KernelType == KernelType.AzureOpenAI;
        IsOpenAI = KernelType == KernelType.OpenAI;
    }

    private void CheckTranslateType()
    {
        IsAzureTranslate = TranslateType == TranslateType.Azure;
        IsBaiduTranslate = TranslateType == TranslateType.Baidu;
    }

    private void CheckSpeechType()
    {
        IsAzureSpeech = SpeechType == SpeechType.Azure;
        IsAzureWhisper = SpeechType == SpeechType.AzureWhisper;
        IsOpenAIWhisper = SpeechType == SpeechType.OpenAIWhisper;
    }

    private void CheckImageType()
    {
        IsAzureImage = ImageGenerateType == ImageGenerateType.AzureDallE;
        IsOpenAIImage = ImageGenerateType == ImageGenerateType.OpenAIDallE;
    }

    partial void OnCurrentStepChanged(int value)
        => CheckStep();

    partial void OnKernelTypeChanged(KernelType value)
        => CheckKernelType();

    partial void OnTranslateTypeChanged(TranslateType value)
        => CheckTranslateType();

    partial void OnSpeechTypeChanged(SpeechType value)
        => CheckSpeechType();

    partial void OnImageGenerateTypeChanged(ImageGenerateType value)
        => CheckImageType();
}
