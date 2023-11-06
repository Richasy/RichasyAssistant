// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 欢迎页面.
/// </summary>
public sealed partial class WelcomePage : WelcomePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomePage"/> class.
    /// </summary>
    public WelcomePage()
    {
        InitializeComponent();
        ViewModel = WelcomePageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        AIPicker.SelectedIndex = (int)ViewModel.KernelType;
        TranslatePicker.SelectedIndex = (int)ViewModel.TranslateType;
        SpeechPicker.SelectedIndex = (int)ViewModel.SpeechType;
        ImagePicker.SelectedIndex = (int)ViewModel.ImageGenerateType;
    }

    private void OnAIKeyBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.TryLoadAIModelSourceCommand.Execute(default);

    private void OnWhisperKeyBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.TryLoadWhisperModelCommand.Execute(default);

    private void OnAIPickerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.KernelType = (KernelType)AIPicker.SelectedIndex;
    }

    private void OnTranslatePickerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.TranslateType = (TranslateType)TranslatePicker.SelectedIndex;
    }

    private void OnSpeechPickerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.SpeechType = (SpeechType)SpeechPicker.SelectedIndex;
    }

    private void OnImagePickerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.ImageGenerateType = (ImageGenerateType)ImagePicker.SelectedIndex;
    }
}

/// <summary>
/// 欢迎页面基类.
/// </summary>
public abstract class WelcomePageBase : PageBase<WelcomePageViewModel>
{
}
