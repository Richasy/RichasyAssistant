// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using RichasyAssistant.App.Controls;
using RichasyAssistant.App.Pages;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Views;
using RichasyAssistant.Models.App.Args;
using Windows.ApplicationModel.Activation;

namespace RichasyAssistant.App.Forms;

/// <summary>
/// 迷你视图.
/// </summary>
public sealed partial class MiniWindow : WindowBase, ITipWindow
{
    private readonly IActivatedEventArgs _launchArgs;
    private bool _isFirstActivate = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniWindow"/> class.
    /// </summary>
    public MiniWindow(IActivatedEventArgs args = default)
    {
        InitializeComponent();
        _launchArgs = args;
        AppWindow.IsShownInSwitchers = false;
        var presenter = AppWindow.Presenter as OverlappedPresenter;
        presenter.SetBorderAndTitleBar(false, false);
        presenter.IsResizable = false;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;

        AppViewModel.Instance.MiniWindow = this;
        Activated += OnMiniWindowActivatedAsync;

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

    private async void OnMiniWindowActivatedAsync(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            Close();
        }
        else if (_isFirstActivate)
        {
            var isFirstLaunch = SettingsToolkit.ReadLocalSetting(SettingNames.IsFirstLaunch, true);
            var isStartup = SettingsToolkit.ReadLocalSetting(SettingNames.IsStartupOpen, true);
            var needHideWhenLaunch = SettingsToolkit.ReadLocalSetting(SettingNames.NeedHideWhenLaunch, false);
            _isFirstActivate = false;
            if (!isFirstLaunch && isStartup && needHideWhenLaunch)
            {
                await Task.Delay(600);
                var notify = new AppNotificationBuilder()
                    .AddText(ResourceToolkit.GetLocalizedString(StringNames.AppInTrayTip))
                    .MuteAudio()
                    .SetDuration(AppNotificationDuration.Default)
                    .BuildNotification();
                AppNotificationManager.Default.Show(notify);
                this.Hide();
            }
            else if (isFirstLaunch)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.IsFirstLaunch, false);
            }
        }
        else if (MiniPageViewModel.Instance.IsInitialized)
        {
            MiniPageViewModel.Instance.InitializeCommand.Execute(default);
        }
    }

    private void OnAppViewModelRequestShowTip(object sender, AppTipNotification e)
    {
        if (e.TargetWindow is ITipWindow window)
        {
            new TipPopup(e.Message, window).ShowAsync(e.Type);
        }
    }
}
