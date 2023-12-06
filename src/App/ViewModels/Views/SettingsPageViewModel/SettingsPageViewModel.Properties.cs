// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 设置页面的视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    private const int BuildYear = 2023;

    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private bool _useMarkdownRenderer;

    [ObservableProperty]
    private string _appVersion;

    [ObservableProperty]
    private string _copyright;
}
