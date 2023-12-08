// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 自定义内核聊天请求.
/// </summary>
public sealed class CustomKernelChatRequest
{
    /// <summary>
    /// 消息.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// 历史记录.
    /// </summary>
    [JsonPropertyName("history")]
    public List<ChatMessage> History { get; set; }

    /// <summary>
    /// 会话选项.
    /// </summary>
    [JsonPropertyName("options")]
    public SessionOptions Options { get; set; }
}
