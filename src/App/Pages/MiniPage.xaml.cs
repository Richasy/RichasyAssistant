// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.Controls.Items;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 迷你页面.
/// </summary>
public sealed partial class MiniPage : MiniPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiniPage"/> class.
    /// </summary>
    public MiniPage()
    {
        InitializeComponent();
        ViewModel = MiniPageViewModel.Instance;
        MainContainer.RegisterPropertyChangedCallback(VisibilityProperty, OnMainVisibilityChanged);
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
        SearchBox.Focus(FocusState.Programmatic);
    }

    private void OnMainVisibilityChanged(DependencyObject sender, DependencyProperty dp)
        => SearchBox?.Focus(FocusState.Programmatic);

    private void OnDeleteItemClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement).DataContext as ChatSessionItemViewModel;
        ViewModel.DeleteSessionCommand.Execute(vm);
    }

    private void OnItemClick(object sender, EventArgs e)
    {
        var vm = (sender as ChatSessionItem).ViewModel;
        ViewModel.OpenSessionCommand.Execute(vm);
    }
}

/// <summary>
/// 迷你页面基类.
/// </summary>
public abstract class MiniPageBase : PageBase<MiniPageViewModel>
{
}
