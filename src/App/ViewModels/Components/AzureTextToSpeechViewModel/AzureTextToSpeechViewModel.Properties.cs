// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Dispatching;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Models.App.Kernel;
using Windows.Media.Playback;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// Azure 语音转文本视图模型.
/// </summary>
public sealed partial class AzureTextToSpeechViewModel
{
    private readonly AzureSpeechKernel _kernel;
    private readonly List<AzureSpeechVoice> _allVoices;
    private readonly MediaPlayer _player;
    private readonly DispatcherQueue _dispatcherQueue;

    private Stream _speechStream;

    [ObservableProperty]
    private Metadata _selectedCulture;

    [ObservableProperty]
    private AzureSpeechVoice _selectedVoice;

    [ObservableProperty]
    private string _text;

    [ObservableProperty]
    private bool _isConverting;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private bool _isAudioEnabled;

    /// <summary>
    /// 支持的语言.
    /// </summary>
    public ObservableCollection<Metadata> SupportCultures { get; }

    /// <summary>
    /// 显示的语音.
    /// </summary>
    public ObservableCollection<AzureSpeechVoice> DisplayVoices { get; }
}
