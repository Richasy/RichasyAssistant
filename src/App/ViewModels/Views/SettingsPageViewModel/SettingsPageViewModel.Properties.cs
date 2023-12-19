// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 设置页面的视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private const int BuildYear = 2023;
    private const string AzureOpenAIId = "AzureOpenAI";
    private const string OpenAIId = "OpenAI";
    private const string AzureTranslateId = "AzureTranslate";
    private const string BaiduTranslateId = "BaiduTranslate";
    private const string AzureDrawId = "AzureDALL·E";
    private const string OpenAIDrawId = "OpenAIDALL·E";
    private const string AzureSpeechId = "AzureSpeech";

    private const string ExtraTranslateFileName = "_extraTranslateServices.json";
    private const string ExtraSpeechFileName = "_extraSpeechServices.json";
    private const string ExtraDrawFileName = "_extraDrawServices.json";

    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private bool _useMarkdownRenderer;

    [ObservableProperty]
    private bool _useStreamOutput;

    [ObservableProperty]
    private string _appVersion;

    [ObservableProperty]
    private string _copyright;

    [ObservableProperty]
    private string _libraryPath;

    [ObservableProperty]
    private int _storageMaxDisplayCount;

    [ObservableProperty]
    private ServiceMetadata _chatKernel;

    [ObservableProperty]
    private ServiceMetadata _drawService;

    [ObservableProperty]
    private ServiceMetadata _translateService;

    [ObservableProperty]
    private ServiceMetadata _speechService;

    /// <summary>
    /// 对话模型服务列表.
    /// </summary>
    public ObservableCollection<ServiceMetadata> ChatKernels { get; }

    /// <summary>
    /// 绘图服务列表.
    /// </summary>
    public ObservableCollection<ServiceMetadata> DrawServices { get; }

    /// <summary>
    /// 翻译服务列表.
    /// </summary>
    public ObservableCollection<ServiceMetadata> TranslateServices { get; }

    /// <summary>
    /// 语音服务列表.
    /// </summary>
    public ObservableCollection<ServiceMetadata> SpeechServices { get; }

    /// <summary>
    /// 内核扩展服务列表.
    /// </summary>
    public ObservableCollection<SlimServiceItemViewModel> KernelExtraServices { get; }

    /// <summary>
    /// 文件搜索最大显示条目数集合.
    /// </summary>
    public ObservableCollection<int> StorageDisplayCountCollection { get; }

    /// <summary>
    /// 内部内核视图模型.
    /// </summary>
    public InternalKernelViewModel InternalKernel { get; }

    /// <summary>
    /// 内部绘图服务视图模型.
    /// </summary>
    public InternalDrawServiceViewModel InternalDrawService { get; }

    /// <summary>
    /// 内部翻译服务视图模型.
    /// </summary>
    public InternalTranslateServiceViewModel InternalTranslate { get; }

    /// <summary>
    /// 内部语音服务视图模型.
    /// </summary>
    public InternalSpeechServiceViewModel InternalSpeech { get; }
}
