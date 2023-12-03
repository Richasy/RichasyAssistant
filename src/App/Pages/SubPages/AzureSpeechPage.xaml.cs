// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages.SubPages;

/// <summary>
/// Azure 语音页面.
/// </summary>
public sealed partial class AzureSpeechPage : AzureSpeechPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSpeechPage"/> class.
    /// </summary>
    public AzureSpeechPage()
    {
        InitializeComponent();
        ViewModel = new AzureSpeechPageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => ViewModel.TextToSpeech?.Dispose();
}

/// <summary>
/// Azure 语音页面基类.
/// </summary>
public abstract class AzureSpeechPageBase : PageBase<AzureSpeechPageViewModel>
{
}
