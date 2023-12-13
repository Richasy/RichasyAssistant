// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.Constants;

/// <summary>
/// 图片生成服务类型.
/// </summary>
public enum DrawType
{
    /// <summary>
    /// Azure Dall-E 服务.
    /// </summary>
    AzureDallE,

    /// <summary>
    /// Open AI Dall-E 服务.
    /// </summary>
    OpenAIDallE,

    /// <summary>
    /// 自定义服务.
    /// </summary>
    Custom,
}
