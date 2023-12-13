// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Dispatching;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// Azure 语音识别视图模型.
/// </summary>
public sealed partial class AzureSpeechRecognizeViewModel
{
    private readonly AzureSpeechKernel _kernel;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly List<string> _cacheTextList;

    [ObservableProperty]
    private bool _isContinuous;

    [ObservableProperty]
    private bool _isRecording;

    [ObservableProperty]
    private string _text;

    [ObservableProperty]
    private Metadata _selectedCulture;

    /// <summary>
    /// 支持的语言.
    /// </summary>
    public ObservableCollection<Metadata> SupportCultures { get; }
}
