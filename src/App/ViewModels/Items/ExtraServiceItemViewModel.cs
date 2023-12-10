// Copyright (c) Richasy Assistant. All rights reserved.

using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.UI.Dispatching;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.App.UI;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 自定义内核项视图模型.
/// </summary>
public sealed partial class ExtraServiceItemViewModel : DataViewModelBase<ServiceMetadata>
{
    private readonly ServiceType _type;
    private readonly DispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    private bool _isLaunching;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isNotStarted;

    [ObservableProperty]
    private bool _isFailed;

    [ObservableProperty]
    private bool _isLaunchingInitializing;

    [ObservableProperty]
    private double _launchingProgress;

    [ObservableProperty]
    private ServiceStatus _status;

    private Process _process;
    private int _servicePort;

    private TaskCompletionSource<ServiceStatus> _launchTaskCompletionSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtraServiceItemViewModel"/> class.
    /// </summary>
    public ExtraServiceItemViewModel(ServiceMetadata data, ServiceType type)
        : base(data)
    {
        _type = type;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Status = ServiceStatus.NotStarted;
        CheckStatus();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if ((_process != null && !_process.HasExited) || IsLaunching)
        {
            return;
        }
        else
        {
            _process?.Dispose();
            _process = null;
        }

        _launchTaskCompletionSource = new TaskCompletionSource<ServiceStatus>();
        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
        var kernelFolderPath = Path.Combine(libPath, "Extensions", _type.ToString(), Data.Id);
        var configPath = Path.Combine(kernelFolderPath, "config.json");
        var config = JsonSerializer.Deserialize<ExtraServiceConfig>(await File.ReadAllTextAsync(configPath));
        var runScript = config.RunScript;

        var tip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.LaunchingKernelTip), config.Name);
        AppViewModel.Instance.ShowTip(tip);

        var uri = new Uri(config.BaseUrl);
        if (!uri.IsDefaultPort)
        {
            _servicePort = uri.Port;
            await AppToolkit.KillProcessIfUsingPortAsync(uri.Port);
        }

        var process = new Process();
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{Path.Combine(kernelFolderPath, runScript)}\"";
        process.StartInfo.WorkingDirectory = kernelFolderPath;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += OnOutputDataReceived;
        process.ErrorDataReceived += OnErrorDataReceived;
        process.Exited += OnProcessExited;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        _process = process;

        LaunchingProgress = 0;
        IsLaunchingInitializing = true;
        Status = ServiceStatus.Launching;
        await _launchTaskCompletionSource.Task;
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        if (_process == null || _servicePort == 0)
        {
            return;
        }

        await AppToolkit.KillProcessIfUsingPortAsync(_servicePort);
        CleanProcess();
        Status = ServiceStatus.NotStarted;
    }

    private void OnProcessExited(object sender, EventArgs e)
        => CleanProcess();

    private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (IsRunning)
            {
                return;
            }

            HandleStandardData(e.Data);
        });
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (IsRunning)
            {
                return;
            }

            HandleStandardData(e.Data);
        });
    }

    private void HandleStandardData(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            Logger.Debug(data);

            if (data.StartsWith("{"))
            {
                var launchInfo = JsonSerializer.Deserialize<ServiceLaunchInfo>(data);
                if (launchInfo != null)
                {
                    Status = launchInfo.Status == ServiceLaunchResult.Success
                        ? ServiceStatus.Running
                        : ServiceStatus.Failed;

                    if (launchInfo.Status == ServiceLaunchResult.Success)
                    {
                        var msg = string.Format(ResourceToolkit.GetLocalizedString(StringNames.KernelLaunchedTip), Data.Name);
                        AppViewModel.Instance.ShowTip(msg, InfoType.Success);
                    }
                    else
                    {
                        var msg = launchInfo.Message;
                        LogException(new Exception(msg));
                        AppViewModel.Instance.ShowTip(msg, InfoType.Error);
                    }
                }
            }
            else
            {
                var pattern = @"(\d+)%";
                var match = Regex.Match(data, pattern);
                if (match.Success)
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        LaunchingProgress = double.Parse(match.Groups[1].Value);
                    });
                }
            }
        }
    }

    private void CheckStatus()
    {
        IsNotStarted = Status == ServiceStatus.NotStarted;
        IsLaunching = Status == ServiceStatus.Launching;
        IsRunning = Status == ServiceStatus.Running;
        IsFailed = Status == ServiceStatus.Failed;

        if ((IsRunning || IsFailed) && _launchTaskCompletionSource != null)
        {
            _launchTaskCompletionSource.TrySetResult(Status);
            _launchTaskCompletionSource = null;
        }
    }

    private void CleanProcess()
    {
        _process?.Dispose();
        _process = null;
    }

    partial void OnStatusChanged(ServiceStatus value)
        => CheckStatus();

    partial void OnLaunchingProgressChanged(double value)
        => IsLaunchingInitializing = value == 0;
}
