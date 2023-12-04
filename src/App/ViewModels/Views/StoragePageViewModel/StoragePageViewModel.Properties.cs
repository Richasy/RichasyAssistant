// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Dispatching;
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

    [ObservableProperty]
    private string _searchText;

    [ObservableProperty]
    private bool _isEverythingAvailable;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isNotStarted;

    [ObservableProperty]
    private StorageSearchTypeItem _currentSearchType;

    /// <summary>
    /// 搜索类型.
    /// </summary>
    public ObservableCollection<StorageSearchTypeItem> SearchTypes { get; }
}
