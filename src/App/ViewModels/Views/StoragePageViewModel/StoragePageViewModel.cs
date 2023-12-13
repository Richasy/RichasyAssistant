// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Dispatching;
using Microsoft.Win32;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Everything.Core;
using RichasyAssistant.Models.App.Local;
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
        Items = new ObservableCollection<StorageItemViewModel>();
        IsGridLayout = SettingsToolkit.ReadLocalSetting(SettingNames.IsStoragePageGridLayout, true);
        SortType = SettingsToolkit.ReadLocalSetting(SettingNames.StorageSortType, StorageSortType.NameAtoZ);

        AttachIsRunningToAsyncCommand(p => IsSearching = p, SearchCommand);
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
    private async Task SearchAsync(string text = null)
    {
        var searchText = string.IsNullOrEmpty(text) ? SearchText : text;
        if (string.IsNullOrEmpty(searchText))
        {
            IsNotStarted = true;
            IsEmpty = false;
            return;
        }

        _lastSearchText = searchText;
        IsEmpty = false;
        IsNotStarted = false;
        var keyword = SearchText;
        var items = new List<StorageItem>();
        var type = CurrentSearchType.Type;
        var totalCount = 0;
        var displayCount = 0;
        TryClear(Items);
        await Task.Run(() =>
        {
            Libs.Everything.Interfaces.IQueryable query = default;
            query = _client
                .Search()
                .Name
                .Contains(keyword);

            query = type switch
            {
                StorageSearchType.Document => query.And.File.Document(),
                StorageSearchType.Audio => query.And.File.Audio(),
                StorageSearchType.Video => query.And.File.Video(),
                StorageSearchType.Image => query.And.File.Picture(),
                StorageSearchType.Zip => query.And.File.Zip(),
                StorageSearchType.Program => query.And.File.Exe(),
                StorageSearchType.Repeat => query.And.File.Duplicates(),
                _ => query,
            };

            var maxCount = SettingsToolkit.ReadLocalSetting(SettingNames.MaxStorageSearchCount, 100);
            totalCount = query.Count();
            foreach (var item in query)
            {
                var storageItem = new StorageItem
                {
                    Name = item.FileName,
                    Path = item.FullPath,
                    CreatedTime = item.Created,
                    LastModifiedTime = item.Modified,
                    ByteLength = item.Size,
                };

                items.Add(storageItem);
            }

            items = GetSortedList(items).ToList();
            if (maxCount > 0)
            {
                items = items.Take(maxCount).ToList();
            }

            displayCount = items.Count();
        });

        foreach (var item in items)
        {
            var vm = new StorageItemViewModel(item);
            Items.Add(vm);
        }

        IsEmpty = Items.Count == 0;

        if (!IsEmpty)
        {
            var templateName = totalCount != displayCount ? StringNames.SearchResultTipWithLimit : StringNames.SearchResultTip;
            var template = ResourceToolkit.GetLocalizedString(templateName);
            var result = totalCount != displayCount
                ? string.Format(template, totalCount, displayCount)
                : string.Format(template, displayCount);
            AppViewModel.Instance.ShowTip(result);
        }
    }

    private List<StorageItem> GetSortedList(List<StorageItem> items)
    {
        var list = items.ToList();
        switch (SortType)
        {
            case StorageSortType.NameAtoZ:
                list = list.OrderBy(x => x.Name).ToList();
                break;
            case StorageSortType.NameZtoA:
                list = list.OrderByDescending(x => x.Name).ToList();
                break;
            case StorageSortType.ModifiedTime:
                list = list.OrderByDescending(x => x.LastModifiedTime).ToList();
                break;
            case StorageSortType.Type:
                list = list.OrderBy(x => x.IsFolder()).ThenBy(x => Path.GetExtension(x.Path)).ThenBy(x => x.Name).ToList();
                break;
            case StorageSortType.SizeLargeToSmall:
                list = list.OrderByDescending(x => x.ByteLength).ToList();
                break;
            case StorageSortType.SizeSmallToLarge:
                list = list.OrderBy(x => x.ByteLength).ToList();
                break;
            default:
                break;
        }

        return list;
    }

    partial void OnCurrentSearchTypeChanged(StorageSearchTypeItem value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.StorageSearchType, value.Type);
        if (!string.IsNullOrEmpty(SearchText) && !IsSearching)
        {
            SearchCommand.Execute(default);
        }
    }

    partial void OnIsGridLayoutChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsStoragePageGridLayout, value);

    partial void OnSortTypeChanged(StorageSortType value)
    {
        if (!string.IsNullOrEmpty(_lastSearchText))
        {
            SearchCommand.Execute(_lastSearchText);
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.StorageSortType, value);
    }
}
