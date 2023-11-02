// Copyright (c) Reader Copilot. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// Chat message structure.
/// </summary>
public sealed class ChatMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessage"/> class.
    /// </summary>
    public ChatMessage()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessage"/> class.
    /// </summary>
    public ChatMessage(ChatMessageRole role, string message, DateTimeOffset time = default, string extension = default)
    {
        Role = role;
        Content = message;
        Time = time == default ? DateTimeOffset.Now : time;
        Extension = extension;
    }

    /// <summary>
    /// Is it a message sent by the user.
    /// </summary>
    public ChatMessageRole Role { get; set; }

    /// <summary>
    /// Message content for display.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Time the message was sent.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// Additional information, such as data sources.
    /// </summary>
    public string Extension { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatMessage message && Time.Equals(message.Time);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Time);
}
