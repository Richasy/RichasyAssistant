// Copyright (c) Richasy Assistant. All rights reserved.

using System.Net.Http;
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
        Link = image.Link;
        Time = image.Time.Humanize();
        TimeDetail = image.Time.ToString("yyyy/MM/dd HH:mm:ss");
    }

    [RelayCommand]
    private void CopyImage()
    {
        var dp = new DataPackage();
        dp.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(Data.Link)));
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
    private Task OpenAsync()
        => Launcher.LaunchUriAsync(new Uri(Data.Link)).AsTask();

    private async Task SaveFileAsync(StorageFile file)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(Data.Link);
        var buffer = await response.Content.ReadAsByteArrayAsync();
        await FileIO.WriteBytesAsync(file, buffer);
    }
}
