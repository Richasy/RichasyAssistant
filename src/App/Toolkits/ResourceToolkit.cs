// Copyright (c) Richasy Assistant. All rights reserved.

using Windows.ApplicationModel.Resources.Core;

namespace RichasyAssistant.App.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public static class ResourceToolkit
{
    /// <summary>
    /// Get localized text.
    /// </summary>
    /// <param name="stringName">Resource name corresponding to localized text.</param>
    /// <returns>Localized text.</returns>
    public static string GetLocalizedString(StringNames stringName)
        => ResourceManager.Current.MainResourceMap[$"Resources/{stringName}"].Candidates[0].ValueAsString;
}
