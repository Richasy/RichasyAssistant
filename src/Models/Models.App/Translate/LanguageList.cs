// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel.DataAnnotations;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.Models.App.Translate;

/// <summary>
/// 语言列表.
/// </summary>
public sealed class LanguageList
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 语言列表.
    /// </summary>
    public List<Metadata> Langauges { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is LanguageList list && Id == list.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
