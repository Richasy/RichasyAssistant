// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部语音服务视图模型.
/// </summary>
public sealed partial class InternalSpeechServiceViewModel : ViewModelBase
{
    [RelayCommand]
    private void Initialize(SpeechType type)
    {
        if (type == SpeechType.Azure)
        {
            AzureSpeechKey = SettingsToolkit.ReadLocalSetting(SettingNames.AzureSpeechKey, string.Empty);
            AzureSpeechRegion = SettingsToolkit.ReadLocalSetting(SettingNames.AzureSpeechRegion, string.Empty);
        }
    }
}
