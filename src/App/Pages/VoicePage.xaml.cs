// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Media.Animation;
using RichasyAssistant.App.Controls;
using RichasyAssistant.App.Pages.SubPages;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 语音页面.
/// </summary>
public sealed partial class VoicePage : VoicePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VoicePage"/> class.
    /// </summary>
    public VoicePage()
    {
        InitializeComponent();
        ViewModel = new VoicePageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        if (ViewModel.SpeechType == SpeechType.Azure)
        {
            SubView.Navigate(typeof(AzureSpeechPage), default, new SuppressNavigationTransitionInfo());
        }
    }
}

/// <summary>
/// 语音页面基类.
/// </summary>
public abstract class VoicePageBase : PageBase<VoicePageViewModel>
{
}
