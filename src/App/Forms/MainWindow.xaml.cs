// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Animation;
using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Args;
using Windows.ApplicationModel.Activation;

namespace RichasyAssistant.App.Forms;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WindowBase, ITipWindow
{
    private readonly IActivatedEventArgs _launchArgs;
    private bool _isActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow(IActivatedEventArgs args = default)
    {
        InitializeComponent();
        _launchArgs = args;
        Closed += OnClosed;
        Activated += OnWindowActivated;

        MinWidth = 800;
        MinHeight = 640;

        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        var lastWidth = SettingsToolkit.ReadLocalSetting(SettingNames.LastWindowWidth, 1000d);
        var lastHeight = SettingsToolkit.ReadLocalSetting(SettingNames.LastWindowHeight, 700d);
        Width = lastWidth;
        Height = lastHeight;

        this.CenterOnScreen();

        AppViewModel.Instance.RequestShowTip += OnAppViewModelRequestShowTip;
        MainViewModel.Instance.RequestNavigate += OnMainViewModelRequestNavigate;
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
    public void ActivateArguments(IActivatedEventArgs e = default)
    {
        e ??= _launchArgs;

        if (e.Kind == ActivationKind.StartupTask)
        {
            _ = this.Hide();
        }
    }

    private void OnAppViewModelRequestShowTip(object sender, AppTipNotification e)
    {
        if (e.TargetWindow is ITipWindow window)
        {
            new TipPopup(e.Message, window).ShowAsync(e.Type);
        }
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        var isMaximized = PInvoke.IsZoomed(new HWND(this.GetWindowHandle()));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastWindowWidth, Width);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastWindowHeight, Height);
        }
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isActivated)
        {
            return;
        }

        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsWindowMaximized, false);
        if (isMaximized)
        {
            (AppWindow.Presenter as OverlappedPresenter).Maximize();
        }

        _isActivated = true;
    }

    private void OnFrameLoaded(object sender, RoutedEventArgs e)
    {
        if (_launchArgs != null)
        {
            ActivateArguments(_launchArgs);
        }

        var lastPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastOpenedPage, FeatureType.Chat);
        MainViewModel.Instance.NavigateTo(lastPage);
    }

    private void OnMainViewModelRequestNavigate(object sender, AppNavigateEventArgs e)
        => MainFrame.Navigate(e.PageType, e.Parameter, new DrillInNavigationTransitionInfo());
}
