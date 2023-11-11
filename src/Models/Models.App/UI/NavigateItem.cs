// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.UI;

/// <summary>
/// 导航项.
/// </summary>
public sealed class NavigateItem
{
    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 功能类型.
    /// </summary>
    public FeatureType Type { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NavigateItem item && Type == item.Type;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type);
}
