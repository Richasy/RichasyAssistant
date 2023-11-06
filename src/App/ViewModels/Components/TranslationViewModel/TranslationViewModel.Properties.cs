// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 翻译视图模型.
/// </summary>
public sealed partial class TranslationViewModel
{
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private Metadata _sourceLanguage;

    [ObservableProperty]
    private Metadata _targetLanguage;

    [ObservableProperty]
    private string _sourceText;

    [ObservableProperty]
    private string _outputText;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private bool _isTranslating;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty]
    private bool _isAvailable;

    /// <summary>
    /// 源语言列表.
    /// </summary>
    public ObservableCollection<Metadata> SourceLanguages { get; }

    /// <summary>
    /// 目标语言列表.
    /// </summary>
    public ObservableCollection<Metadata> TargetLanguages { get; }
}
