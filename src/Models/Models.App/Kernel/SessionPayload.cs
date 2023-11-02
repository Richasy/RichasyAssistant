// Copyright (c) Reader Copilot. All rights reserved.

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 会话负载.
/// </summary>
public sealed class SessionPayload
{
    /// <summary>
    /// 会话标识符.
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// 历史消息记录.
    /// </summary>
    public List<ChatMessage> Messages { get; set; }

    /// <summary>
    /// 会话设置.
    /// </summary>
    public SessionOptions Options { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SessionPayload payload && SessionId == payload.SessionId;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(SessionId);
}
