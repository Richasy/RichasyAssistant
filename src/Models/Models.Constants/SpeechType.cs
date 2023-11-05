// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.Constants;

/// <summary>
/// 语音服务类型.
/// </summary>
public enum SpeechType
{
    /// <summary>
    /// Azure 语音服务.
    /// </summary>
    Azure,

    /// <summary>
    /// Azure Whisper 语音服务.
    /// </summary>
    AzureWhisper,

    /// <summary>
    /// Open AI 语音服务.
    /// </summary>
    OpenAIWhisper,
}
