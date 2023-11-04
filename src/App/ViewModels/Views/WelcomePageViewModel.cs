// Copyright (c) Richasy Assistant. All rights reserved.

using System.Diagnostics;
using RichasyAssistant.App.ViewModels.Components;
using Windows.Storage;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 欢迎页面视图模型.
/// </summary>
public sealed partial class WelcomePageViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private int _stepCount;

    [ObservableProperty]
    private bool _isFREStep;

    [ObservableProperty]
    private bool _isLibraryStep;

    [ObservableProperty]
    private bool _isAIStep;

    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomePageViewModel"/> class.
    /// </summary>
    private WelcomePageViewModel()
    {
        StepCount = 3;
        CurrentStep = 0;
        CheckStep();
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static WelcomePageViewModel Instance { get; } = new();

    [RelayCommand]
    private static void Restart()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.SkipWelcome, true);
        Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().UnregisterKey();
        Application.Current.Exit();
        var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c start ricass://";
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
    }

    [RelayCommand]
    private void GoNext()
    {
        if (CurrentStep < StepCount - 1)
        {
            CurrentStep++;
        }
        else
        {
            Restart();
        }
    }

    [RelayCommand]
    private async Task CreateLibraryAsync()
    {
        var folderObj = await FileToolkit.PickFolderAsync(AppViewModel.Instance.ActivatedWindow);
        if (folderObj is StorageFolder folder)
        {
            var hasFiles = Directory.GetFiles(folder.Path).Length > 0;
            if (hasFiles)
            {
                AppViewModel.Instance.ShowTip(StringNames.FolderMustEmpty, InfoType.Error);
                return;
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.LibraryFolderPath, folder.Path);
            GoNext();
        }
    }

    [RelayCommand]
    private async Task OpenLibraryAsync()
    {
        var folderObj = await FileToolkit.PickFolderAsync(AppViewModel.Instance.ActivatedWindow);
        if (folderObj is StorageFolder folder)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LibraryFolderPath, folder.Path);
            var secretFilePath = Path.Combine(folder.Path, "_secret_.db");
            if (!File.Exists(secretFilePath))
            {
                // 没有可用的配置文件，仍需要完成剩余配置.
                GoNext();
            }
            else
            {
                Restart();
            }
        }
    }

    private void CheckStep()
    {
        IsFREStep = CurrentStep == 0;
        IsLibraryStep = CurrentStep == 1;
        IsAIStep = CurrentStep == 2;
    }

    partial void OnCurrentStepChanged(int value)
        => CheckStep();
}
