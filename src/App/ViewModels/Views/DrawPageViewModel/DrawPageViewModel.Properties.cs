// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using RichasyAssistant.Libs.Kernel.DrawKernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 绘图页面视图模型.
/// </summary>
public sealed partial class DrawPageViewModel
{
    private readonly DrawKernel _kernel;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private string _imagePath;

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string _poweredBy;

    [ObservableProperty]
    private OpenAIImageSize _size;
}
