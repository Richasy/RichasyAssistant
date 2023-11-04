// Copyright (c) Richasy Assistant. All rights reserved.

using H.NotifyIcon;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.Windows.AppLifecycle;
using NLog;
using RichasyAssistant.App.Forms;
using RichasyAssistant.App.ViewModels.Components;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using WinRT.Interop;

namespace RichasyAssistant.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// 应用标识符.
    /// </summary>
    public const string Id = "084C510A-A128-4709-9FFE-81F6A1B3F58F";
    private DispatcherQueue _dispatcherQueue;
    private WindowBase _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        var mainAppInstance = AppInstance.FindOrRegisterForKey(Id);
        mainAppInstance.Activated += OnAppInstanceActivated;
        UnhandledException += OnUnhandledException;
    }

    private TaskbarIcon TrayIcon { get; set; }

    private bool HandleCloseEvents { get; set; } = true;

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var instance = AppInstance.FindOrRegisterForKey(Id);
        var eventArgs = instance.GetActivatedEventArgs();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        var rootFolder = ApplicationData.Current.LocalFolder;
        var fullPath = $"{rootFolder.Path}\\Logger\\";
        NLog.GlobalDiagnosticsContext.Set("LogPath", fullPath);

        var data = eventArgs.Data is IActivatedEventArgs
            ? eventArgs.Data as IActivatedEventArgs
            : args.UWPLaunchActivatedEventArgs;

        NLog.LogManager.GetCurrentClassLogger().Info($"App Launched: {data?.Kind ?? ActivationKind.Launch}");

        LaunchWindow(data);
    }

    /// <summary>
    /// Try activating the window and bringing it to the foreground.
    /// </summary>
    private void ActivateWindow(AppActivationArguments arguments = default)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            var needWelcome = !SettingsToolkit.ReadLocalSetting(SettingNames.SkipWelcome, false);
            if (needWelcome)
            {
                return;
            }

            if (_window == null)
            {
                LaunchWindow();
            }
            else if (_window.Visible && HandleCloseEvents && arguments?.Data == null)
            {
                _window.Hide();
            }
            else
            {
                _window.Activate();
                _window.SetForegroundWindow();
            }

            try
            {
                if (arguments?.Data is IActivatedEventArgs args)
                {
                    ((MiniWindow)AppViewModel.Instance.MiniWindow).ActivateArgumentsAsync(args);
                }
            }
            catch (Exception)
            {
            }
        });
    }

    private void InitializeTrayIcon()
    {
        if (TrayIcon != null)
        {
            return;
        }

        var showHideWindowCommand = (XamlUICommand)Resources["ShowHideWindowCommand"];
        showHideWindowCommand.ExecuteRequested += OnShowHideWindowCommandExecuteRequested;

        var exitApplicationCommand = (XamlUICommand)Resources["QuitCommand"];
        exitApplicationCommand.ExecuteRequested += OnQuitCommandExecuteRequested;

        try
        {
            TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
            TrayIcon.ForceCreate();
        }
        catch (Exception)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Error("Failed to initialize tray icon");
        }
    }

    private void LaunchWindow(IActivatedEventArgs args = default)
    {
        var needWelcome = !SettingsToolkit.ReadLocalSetting(SettingNames.SkipWelcome, false);

        if (needWelcome)
        {
            var window = new WelcomeWindow();
            window.Activate();
        }
        else
        {
            _window = new MiniWindow(args);
            HideWindowFromTaskBar();
            MoveAndResize();
            _window.Closed += OnMainWindowClosed;
            if (HandleCloseEvents)
            {
                InitializeTrayIcon();
            }

            _window.Activate();
        }
    }

    private void OnMainWindowClosed(object sender, WindowEventArgs args)
    {
        if (HandleCloseEvents)
        {
            args.Handled = true;
            _window.Hide();
        }
    }

    private void HideWindowFromTaskBar()
    {
        var hwnd = new HWND(WindowNative.GetWindowHandle(_window));

        PInvoke.ShowWindow(hwnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_HIDE);
        var flags = PInvoke.GetWindowLong(hwnd, Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(hwnd, Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, flags | 0x00000080);
        PInvoke.ShowWindow(hwnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_SHOW);
    }

    private Windows.Graphics.RectInt32 GetRenderRect(DisplayArea displayArea)
    {
        var scaleFactor = _window.GetDpiForWindow() / 96d;
        var width = Convert.ToInt32(400 * scaleFactor);
        var height = Convert.ToInt32(600 * scaleFactor);
        var workArea = displayArea.WorkArea;
        var left = (workArea.Width - width) / 2d;
        var top = workArea.Height - height - (12 * scaleFactor);
        return new Windows.Graphics.RectInt32(Convert.ToInt32(left), Convert.ToInt32(top), width, height);
    }

    private void MoveAndResize()
    {
        var hwnd = WindowNative.GetWindowHandle(_window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
        if (displayArea != null)
        {
            var rect = GetRenderRect(displayArea);
            var scaleFactor = _window.GetDpiForWindow() / 96d;
            _window.MinWidth = 400;
            _window.MaxWidth = 400;
            _window.MinHeight = 100;

            var maxHeight = (displayArea.WorkArea.Height / scaleFactor) + 16;
            _window.MaxHeight = maxHeight < rect.Height ? maxHeight : rect.Height;
            _window.AppWindow.MoveAndResize(rect);
        }
    }

    private void ExitApp()
    {
        HandleCloseEvents = false;
        TrayIcon?.Dispose();
        _window?.Close();

        AppViewModel.BeforeExitAsync().Wait();
        Environment.Exit(0);
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Error(e.Exception, "An exception occurred while the application was running");
        e.Handled = true;
    }

    private void OnQuitCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ExitApp();

    private void OnShowHideWindowCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ActivateWindow();

    private void OnAppInstanceActivated(object sender, AppActivationArguments e)
        => ActivateWindow(e);
}
