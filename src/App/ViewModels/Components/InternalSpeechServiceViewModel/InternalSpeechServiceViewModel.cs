// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Locator;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部语音服务视图模型.
/// </summary>
public sealed partial class InternalSpeechServiceViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalSpeechServiceViewModel"/> class.
    /// </summary>
    public InternalSpeechServiceViewModel()
    {
        AzureWhisperModels = new ObservableCollection<string>();
        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand, TryLoadModelSourceCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync(SpeechType type)
    {
        if (type == SpeechType.Azure)
        {
            AzureSpeechKey = SettingsToolkit.ReadLocalSetting(SettingNames.AzureSpeechKey, string.Empty);
            AzureSpeechRegion = SettingsToolkit.ReadLocalSetting(SettingNames.AzureSpeechRegion, string.Empty);
        }
        else if (type == SpeechType.AzureWhisper)
        {
            AzureWhisperKey = SettingsToolkit.ReadLocalSetting(SettingNames.AzureWhisperKey, string.Empty);
            AzureWhisperEndpoint = SettingsToolkit.ReadLocalSetting(SettingNames.AzureWhisperEndpoint, string.Empty);
            await TryLoadModelSourceAsync(type);
        }
        else if (type == SpeechType.OpenAIWhisper)
        {
            OpenAIWhisperKey = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAIWhisperKey, string.Empty);
            OpenAICustomEndpoint = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
        }
    }

    [RelayCommand]
    private async Task TryLoadModelSourceAsync(SpeechType type)
    {
        if (type != SpeechType.AzureWhisper
            || (type == SpeechType.AzureWhisper && AzureWhisperModels.Count > 0))
        {
            return;
        }

        try
        {
            GlobalSettings.Set(SettingNames.AzureWhisperKey, AzureWhisperKey);
            GlobalSettings.Set(SettingNames.AzureWhisperEndpoint, AzureWhisperEndpoint);

            // TODO: 加载模型.
            await Task.Delay(200);
            TryClear(AzureWhisperModels);
            AzureWhisperModels.Add("whisper");
            var localModelName = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultAzureWhisperModelName, string.Empty);
            AzureWhisperModelName = string.IsNullOrEmpty(localModelName)
                ? AzureWhisperModels.FirstOrDefault()
                : AzureWhisperModels.FirstOrDefault(p => p.Equals(localModelName));
        }
        catch (Exception ex)
        {
            LogException(ex);
        }
    }
}
