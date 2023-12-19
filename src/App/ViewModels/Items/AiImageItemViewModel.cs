// Copyright (c) Richasy Assistant. All rights reserved.

using Humanizer;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// AI图片项视图模型.
/// </summary>
public sealed partial class AiImageItemViewModel : DataViewModelBase<AiImage>
{
    [ObservableProperty]
    private string _link;

    [ObservableProperty]
    private string _time;

    [ObservableProperty]
    private string _timeDetail;

    /// <summary>
    /// Initializes a new instance of the <see cref="AiImageItemViewModel"/> class.
    /// </summary>
    public AiImageItemViewModel(AiImage image)
        : base(image)
    {
        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
        Link = Path.Combine(libPath, image.Link);
        Time = image.Time.Humanize();
        TimeDetail = image.Time.ToString("yyyy/MM/dd HH:mm:ss");
    }

    [RelayCommand]
    private async Task CopyImageAsync()
    {
        var dp = new DataPackage();
        var file = await StorageFile.GetFileFromPathAsync(Link);
        dp.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(StringNames.Copied, InfoType.Success);
    }

    [RelayCommand]
    private void CopyPrompt()
    {
        var dp = new DataPackage();
        dp.SetText(Data.Prompt);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(StringNames.Copied, InfoType.Success);
    }

    [RelayCommand]
    private async Task SaveAsAsync()
    {
        try
        {
            var file = await FileToolkit.SaveFileAsync(".png", $"{DateTime.Now:yyyy-mm-dd_HH_mm_ss}.png", AppViewModel.Instance.ActivatedWindow);
            if (file != null)
            {
                await SaveFileAsync(file);
                AppViewModel.Instance.ShowTip(StringNames.FileSaved, InfoType.Success);
            }
        }
        catch (Exception)
        {
            AppViewModel.Instance.ShowTip(StringNames.SaveFileFailed, InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task OpenAsync()
    {
        var imageFile = await StorageFile.GetFileFromPathAsync(Link);
        await Launcher.LaunchFileAsync(imageFile).AsTask();
    }

    private async Task SaveFileAsync(StorageFile file)
    {
        var imageFile = await StorageFile.GetFileFromPathAsync(Link);
        await imageFile.CopyAndReplaceAsync(file);
    }
}
