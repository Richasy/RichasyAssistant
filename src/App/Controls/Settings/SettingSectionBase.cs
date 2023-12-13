// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Controls.Settings;

/// <summary>
/// 设置页面控件的基类.
/// </summary>
public abstract class SettingSectionBase : ReactiveUserControl<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingSectionBase"/> class.
    /// </summary>
    public SettingSectionBase() => IsTabStop = false;
}
