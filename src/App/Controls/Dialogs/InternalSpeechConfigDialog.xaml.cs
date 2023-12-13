// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Dialogs;

/// <summary>
/// 内部语音服务配置对话框.
/// </summary>
public sealed partial class InternalSpeechConfigDialog : ContentDialog
{
    private readonly SpeechType _type;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalSpeechConfigDialog"/> class.
    /// </summary>
    public InternalSpeechConfigDialog(InternalSpeechServiceViewModel viewModel, SpeechType type)
    {
        InitializeComponent();
        ViewModel = viewModel;
        _type = type;
        IsAzureSpeech = type == SpeechType.Azure;
        IsAzureWhisper = type == SpeechType.AzureWhisper;
        IsOpenAIWhisper = type == SpeechType.OpenAIWhisper;
        AppToolkit.ResetControlTheme(this);
        Loaded += OnLoaded;
    }

    private bool IsAzureSpeech { get; }

    private bool IsAzureWhisper { get; }

    private bool IsOpenAIWhisper { get; }

    private InternalSpeechServiceViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(_type);
}
