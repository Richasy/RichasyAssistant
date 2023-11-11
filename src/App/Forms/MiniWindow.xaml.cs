// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Animation;
using RichasyAssistant.App.Controls;
using RichasyAssistant.App.Pages;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Args;
using Windows.ApplicationModel.Activation;

namespace RichasyAssistant.App.Forms;

/// <summary>
/// 迷你视图.
/// </summary>
public sealed partial class MiniWindow : WindowBase, ITipWindow
{
    private readonly IActivatedEventArgs _launchArgs;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniWindow"/> class.
    /// </summary>
    public MiniWindow(IActivatedEventArgs args = default)
    {
        InitializeComponent();
        _launchArgs = args;
        AppWindow.IsShownInSwitchers = false;

        Width = 400;
        Height = 600;
        this.CenterOnScreen();

        AppViewModel.Instance.RequestShowTip += OnAppViewModelRequestShowTip;
        MainFrame.Navigate(typeof(MiniPage), default, new DrillInNavigationTransitionInfo());
    }

    /// <inheritdoc/>
    public async Task ShowTipAsync(UIElement element, double delaySeconds)
    {
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(element);
        element.Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        element.Visibility = Visibility.Collapsed;
        _ = TipContainer.Children.Remove(element);
        if (TipContainer.Children.Count == 0)
        {
            TipContainer.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 激活参数.
    /// </summary>
    /// <param name="e">参数.</param>
    public async void ActivateArgumentsAsync(IActivatedEventArgs e = default)
    {
        e ??= _launchArgs;

        if (e.Kind == ActivationKind.StartupTask)
        {
            this.Hide();
        }

        await Task.CompletedTask;
    }

    private void OnAppViewModelRequestShowTip(object sender, AppTipNotification e)
    {
        if (e.TargetWindow is ITipWindow window)
        {
            new TipPopup(e.Message, window).ShowAsync(e.Type);
        }
    }
}
