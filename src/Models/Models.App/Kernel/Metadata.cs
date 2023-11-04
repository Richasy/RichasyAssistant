// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 元数据.
/// </summary>
public sealed class Metadata
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 值.
    /// </summary>
    public string Value { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Metadata metadata && Id == metadata.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
