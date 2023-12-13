// Copyright (c) Richasy Assistant. All rights reserved.

using System.Diagnostics;
using Humanizer;
using NeoSmart.PrettySize;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Local;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 存储项视图模型.
/// </summary>
public sealed partial class StorageItemViewModel : DataViewModelBase<StorageItem>
{
    [ObservableProperty]
    private string _fileSize;

    [ObservableProperty]
    private bool _isFolder;

    [ObservableProperty]
    private string _modifiedTime;

    [ObservableProperty]
    private string _modifiedDisplayText;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageItemViewModel"/> class.
    /// </summary>
    /// <param name="data">数据.</param>
    public StorageItemViewModel(StorageItem data)
        : base(data)
    {
        IsFolder = data.IsFolder();
        if (!IsFolder && data.ByteLength > 0)
        {
            FileSize = PrettySize.Bytes(data.ByteLength).Format(UnitBase.Base10, UnitStyle.Abbreviated);
        }

        ModifiedDisplayText = data.LastModifiedTime.Humanize();
        ModifiedTime = data.LastModifiedTime.ToString("yyyy/MM/dd HH:mm:ss");
    }

    [RelayCommand]
    private void Open()
    {
        try
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = Data.Path;
            process.Start();
        }
        catch (Exception ex)
        {
            LogException(ex);
            AppViewModel.Instance.ShowTip(StringNames.OpenItemFailed, InfoType.Error);
        }
    }

    [RelayCommand]
    private void OpenInFileExplorer()
    {
        try
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = IsFolder
                ? Data.Path
                : Directory.GetParent(Data.Path).FullName;

            process.Start();
        }
        catch (Exception ex)
        {
            LogException(ex);
            AppViewModel.Instance.ShowTip(StringNames.OpenItemFailed, InfoType.Error);
        }
    }

    [RelayCommand]
    private void CopyPath()
    {
        var dp = new DataPackage();
        dp.SetText(Data.Path);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(StringNames.Copied, InfoType.Success);
    }

    [RelayCommand]
    private async Task OpenWithAsync()
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(Data.Path);
            var options = new LauncherOptions()
            {
                DisplayApplicationPicker = true,
            };
            await Launcher.LaunchFileAsync(file, options);
        }
        catch (Exception ex)
        {
            LogException(ex);
            AppViewModel.Instance.ShowTip(StringNames.OpenItemFailed, InfoType.Error);
        }
    }
}
