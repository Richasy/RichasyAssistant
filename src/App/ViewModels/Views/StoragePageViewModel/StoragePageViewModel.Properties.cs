// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Dispatching;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Everything.Core;
using RichasyAssistant.Models.App.UI;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 文件存储页面的视图模型.
/// </summary>
public sealed partial class StoragePageViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private Everything _client;
    private string _lastSearchText;

    [ObservableProperty]
    private string _searchText;

    [ObservableProperty]
    private bool _isEverythingAvailable;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isNotStarted;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private StorageSearchTypeItem _currentSearchType;

    [ObservableProperty]
    private bool _isGridLayout;

    [ObservableProperty]
    private StorageSortType _sortType;

    /// <summary>
    /// 搜索类型.
    /// </summary>
    public ObservableCollection<StorageSearchTypeItem> SearchTypes { get; }

    /// <summary>
    /// 搜索结果.
    /// </summary>
    public ObservableCollection<StorageItemViewModel> Items { get; }
}
