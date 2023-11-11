// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// App 标题栏.
/// </summary>
public sealed partial class AppTitleBar : AppTitleBarBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppTitleBar"/> class.
    /// </summary>
    public AppTitleBar()
    {
        InitializeComponent();
        ViewModel = AppViewModel.Instance;
    }
}

/// <summary>
/// App标题栏基类.
/// </summary>
public abstract class AppTitleBarBase : ReactiveUserControl<AppViewModel>
{
}
