// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// Whisper 内核.
/// </summary>
public sealed partial class WhisperKernel
{
    /// <summary>
    /// 检查配置是否有效.
    /// </summary>
    /// <param name="type">语音类型.</param>
    /// <returns>是否有效.</returns>
    public static bool IsConfigValid(SpeechType type)
    {
        if (type == SpeechType.AzureWhisper)
        {
            return !string.IsNullOrEmpty(GlobalSettings.TryGet<string>(SettingNames.AzureWhisperKey))
                && !string.IsNullOrEmpty(GlobalSettings.TryGet<string>(SettingNames.AzureWhisperEndpoint));
        }
        else if (type == SpeechType.OpenAIWhisper)
        {
            return !string.IsNullOrEmpty(GlobalSettings.TryGet<string>(SettingNames.OpenAIWhisperKey));
        }
        else
        {
            var customModelId = GlobalSettings.TryGet<string>(SettingNames.CustomSpeechId);
            return !string.IsNullOrEmpty(customModelId);
        }
    }
}
