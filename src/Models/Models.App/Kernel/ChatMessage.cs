// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
    public ChatMessage(ChatMessageRole role, string message, DateTimeOffset time = default, string assistantId = default, string extension = default)
    {
        Role = role;
        Content = message;
        Time = time == default ? DateTimeOffset.Now : time;
        AssistantId = assistantId;
        Extension = extension;
        Id = Time.ToUnixTimeMilliseconds().ToString();
    }

    /// <summary>
    /// Is it a message sent by the user.
    /// </summary>
    [JsonPropertyName("role")]
    public ChatMessageRole Role { get; set; }

    /// <summary>
    /// Message content for display.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// Time the message was sent.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// Additional information, such as data sources.
    /// </summary>
    [JsonPropertyName("extension")]
    public string? Extension { get; set; }

    /// <summary>
    /// Assistant identifier.
    /// </summary>
    [JsonPropertyName("assistant_id")]
    public string? AssistantId { get; set; }

    /// <summary>
    /// 标识符.
    /// </summary>
    [Key]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatMessage message && Id == message.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
