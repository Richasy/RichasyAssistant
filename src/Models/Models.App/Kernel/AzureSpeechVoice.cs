// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// Azure 语音声音.
/// </summary>
public sealed class AzureSpeechVoice
{
    /// <summary>
    /// Voice name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Voice id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Is it a female voice.
    /// </summary>
    public bool IsFemale { get; set; }

    /// <summary>
    /// Is it a neural voice.
    /// </summary>
    public bool IsNeural { get; set; }

    /// <summary>
    /// Voice region.
    /// </summary>
    public string Locale { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AzureSpeechVoice metadata && Id == metadata.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
