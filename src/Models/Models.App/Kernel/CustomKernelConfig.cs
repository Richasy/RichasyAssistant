// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 自定义内核配置.
/// </summary>
public sealed class CustomKernelConfig : ExtraServiceConfig
{
    /// <summary>
    /// 是否支持流输出.
    /// </summary>
    [JsonPropertyName("support_stream_output")]
    public bool SupportStreamOutput { get; set; }

    /// <summary>
    /// 基础聊天（非流式输出）.
    /// </summary>
    [JsonPropertyName("basic_chat")]
    public string BasicChat { get; set; }

    /// <summary>
    /// 基础聊天（流式输出）.
    /// </summary>
    [JsonPropertyName("stream_chat")]
    public string StreamChat { get; set; }

    /// <summary>
    /// 是否是工具.
    /// </summary>
    [JsonPropertyName("is_tool")]
    public bool IsTool { get; set; }
}
