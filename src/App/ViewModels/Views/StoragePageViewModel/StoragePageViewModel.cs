// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Win32;
using RichasyAssistant.App.Extensions;
using RichasyAssistant.Libs.Everything.Core;
using RichasyAssistant.Models.App.UI;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 文件存储页面的视图模型.
/// </summary>
public sealed partial class StoragePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoragePageViewModel"/> class.
    /// </summary>
    public StoragePageViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        SearchTypes = new ObservableCollection<StorageSearchTypeItem>();
    }

    private static StorageSearchTypeItem CreateSearchTypeItem(StorageSearchType type)
    {
        var item = new StorageSearchTypeItem();
        item.Type = type;
        switch (type)
        {
            case StorageSearchType.Generic:
                item.Name = ResourceToolkit.GetLocalizedString(StringNames.GenericSearch);
                item.Icon = FluentSymbol.Search;
                break;
            case StorageSearchType.Document:
                item.Name = ResourceToolkit.GetLocalizedString(StringNames.DocumentSearch);
                item.Icon = FluentSymbol.Document;
                break;
            case StorageSearchType.Audio:
                item.Name = ResourceToolkit.GetLocalizedString(StringNames.AudioSearch);
                item.Icon = FluentSymbol.MusicNote1;
                break;
            case StorageSearchType.Video:
                item.Name = ResourceToolkit.GetLocalizedString(StringNames.VideoSearch);
                item.Icon = FluentSymbol.Video;
                break;
            case StorageSearchType.Image:
                item.Name = ResourceToolkit.GetLocalizedString(StringNames.ImageSearch);
                item.Icon = FluentSymbol.Image;
                break;
            case StorageSearchType.Zip:
                item.Name = ResourceToolkit.GetLocalizedString(StringNames.ZipSearch);
                item.Icon = FluentSymbol.FolderZip;
                break;
            case StorageSearchType.Program:
                item.Name = ResourceToolkit.GetLocalizedString(StringNames.ProgramSearch);
                item.Icon = FluentSymbol.WindowBulletList;
                break;
            case StorageSearchType.Repeat:
                item.Name = ResourceToolkit.GetLocalizedString(StringNames.RepeatFile);
                item.Icon = FluentSymbol.PositionBackward;
                break;
            default:
                break;
        }

        return item;
    }

    [RelayCommand]
    private void Initialize()
    {
        if (_client != null)
        {
            return;
        }

        var appPath = string.Empty;
        try
        {
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Everything")?.GetValue("DisplayIcon");
            appPath = key is string location ? location : string.Empty;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Attempt to get Everything install location failed");
        }

        _client = new Everything(appPath);

        try
        {
            var major = _client.GetMajorVersion();
            IsEverythingAvailable = major > 0;

            SearchTypes.Add(CreateSearchTypeItem(StorageSearchType.Generic));
            SearchTypes.Add(CreateSearchTypeItem(StorageSearchType.Document));
            SearchTypes.Add(CreateSearchTypeItem(StorageSearchType.Audio));
            SearchTypes.Add(CreateSearchTypeItem(StorageSearchType.Video));
            SearchTypes.Add(CreateSearchTypeItem(StorageSearchType.Image));
            SearchTypes.Add(CreateSearchTypeItem(StorageSearchType.Zip));
            SearchTypes.Add(CreateSearchTypeItem(StorageSearchType.Program));
            SearchTypes.Add(CreateSearchTypeItem(StorageSearchType.Repeat));

            var lastSearchType = SettingsToolkit.ReadLocalSetting(SettingNames.StorageSearchType, StorageSearchType.Generic);
            CurrentSearchType = SearchTypes.FirstOrDefault(p => p.Type == lastSearchType);
            IsNotStarted = true;
        }
        catch (Exception)
        {
            IsEverythingAvailable = false;
            _client = default;
        }
    }

    [RelayCommand]
    private async Task TestAsync()
    {
        var query = _client.Search()
                    .Name
                    .Contains("msbuild.exe");
        var first = query.First(p => p.Size > 0);
        var path = first.FullPath;
        using var thumbnail = WindowsThumbnailProvider.GetThumbnail(path, 256, 256, ThumbnailOptions.None);
        using var ms = new MemoryStream();
        thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        ms.Seek(0, SeekOrigin.Begin);
        var image = new BitmapImage();
        await image.SetSourceAsync(ms.AsRandomAccessStream());
    }

    partial void OnCurrentSearchTypeChanged(StorageSearchTypeItem value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.StorageSearchType, value.Type);
}
