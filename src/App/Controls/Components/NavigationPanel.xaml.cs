// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 导航面板.
/// </summary>
public sealed partial class NavigationPanel : NavigationPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPanel"/> class.
    /// </summary>
    public NavigationPanel()
    {
        InitializeComponent();
        ViewModel = MainViewModel.Instance;
    }
}

/// <summary>
/// 导航面板基类.
/// </summary>
public abstract class NavigationPanelBase : ReactiveUserControl<MainViewModel>
{
}
