// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel.DrawKernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 绘图页面视图模型.
/// </summary>
public sealed partial class DrawPageViewModel
{
    private DrawKernel _kernel;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string _poweredBy;

    [ObservableProperty]
    private OpenAIImageSize _size;

    [ObservableProperty]
    private double _historyColumnWidth;

    [ObservableProperty]
    private AiImageItemViewModel _currentImage;

    [ObservableProperty]
    private bool _isHistoryEmpty;

    [ObservableProperty]
    private int _historyPageIndex;

    [ObservableProperty]
    private bool _historyHasMore;

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    private string _errorText;

    /// <summary>
    /// 历史记录.
    /// </summary>
    public ObservableCollection<AiImageItemViewModel> History { get; }
}
