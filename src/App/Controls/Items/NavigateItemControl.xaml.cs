// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 导航项控件.
/// </summary>
public sealed partial class NavigateItemControl : NavigateItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItemControl"/> class.
    /// </summary>
    public NavigateItemControl() => InitializeComponent();

    private void OnNavItemClick(object sender, RoutedEventArgs e)
    {
        if (MainViewModel.Instance.CurrentFeature == ViewModel.Data.Type)
        {
            return;
        }

        MainViewModel.Instance.NavigateTo(ViewModel.Data.Type);
    }
}

/// <summary>
/// 导航项控件基类.
/// </summary>
public abstract class NavigateItemControlBase : ReactiveUserControl<NavigateItemViewModel>
{
}
