// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.UI;

/// <summary>
/// 存储搜索类型.
/// </summary>
public sealed class StorageSearchTypeItem
{
    /// <summary>
    /// 搜索类型.
    /// </summary>
    public StorageSearchType Type { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 图标.
    /// </summary>
    public FluentSymbol Icon { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is StorageSearchTypeItem item && Type == item.Type;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type);
}
