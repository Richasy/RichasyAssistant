// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.Controls.Settings;

/// <summary>
/// 文件搜索设置.
/// </summary>
public sealed partial class StorageSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageSettingSection"/> class.
    /// </summary>
    public StorageSettingSection() => InitializeComponent();

    internal static string GetCountText(int value)
    {
        return value == -1
            ? ResourceToolkit.GetLocalizedString(StringNames.NoLimit)
            : string.Format(ResourceToolkit.GetLocalizedString(StringNames.CountLimitTemplate), value);
    }
}
