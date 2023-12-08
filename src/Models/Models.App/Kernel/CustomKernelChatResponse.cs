// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 自定义内核聊天响应.
/// </summary>
public sealed class CustomKernelChatResponse
{
    /// <summary>
    /// 消息内容.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// 是否完成.
    /// </summary>
    [JsonPropertyName("is_finish")]
    public bool IsFinish { get; set; }

    /// <summary>
    /// 扩展内容.
    /// </summary>
    [JsonPropertyName("extension")]
    public string Extension { get; set; }
}
