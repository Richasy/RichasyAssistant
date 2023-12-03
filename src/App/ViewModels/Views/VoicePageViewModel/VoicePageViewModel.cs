// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 语音页面视图模型.
/// </summary>
public sealed partial class VoicePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VoicePageViewModel"/> class.
    /// </summary>
    public VoicePageViewModel()
    {
        SpeechType = SettingsToolkit.IsSettingKeyExist(SettingNames.CurrentSpeechType)
            ? SettingsToolkit.ReadLocalSetting(SettingNames.CurrentSpeechType, SpeechType.Azure)
            : SettingsToolkit.ReadLocalSetting(SettingNames.DefaultSpeech, SpeechType.Azure);

        var serviceName = SpeechType switch
        {
            SpeechType.Azure => ResourceToolkit.GetLocalizedString(StringNames.AzureSpeech),
            SpeechType.AzureWhisper => ResourceToolkit.GetLocalizedString(StringNames.AzureWhisper),
            SpeechType.OpenAIWhisper => ResourceToolkit.GetLocalizedString(StringNames.OpenAIWhisper),
            _ => throw new NotImplementedException(),
        };

        PoweredBy = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PoweredBy), serviceName);
    }
}
