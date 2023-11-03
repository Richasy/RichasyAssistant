// Copyright (c) Reader Copilot. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 会话负载.
/// </summary>
public sealed class SessionPayload
{
    /// <summary>
    /// 会话标识符.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 历史消息记录.
    /// </summary>
    public List<ChatMessage> Messages { get; set; }

    /// <summary>
    /// 会话设置.
    /// </summary>
    public SessionOptions Options { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SessionPayload payload && Id == payload.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
