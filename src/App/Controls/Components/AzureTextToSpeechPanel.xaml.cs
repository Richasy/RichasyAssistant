// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using Windows.Storage;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// Azure 文本转语音面板.
/// </summary>
public sealed partial class AzureTextToSpeechPanel : AzureTextToSpeechPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTextToSpeechPanel"/> class.
    /// </summary>
    public AzureTextToSpeechPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => InputBox.Focus(FocusState.Programmatic);

    private async void OnSaveButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var fileName = $"{DateTime.Now:yyyy-mm-dd_HH_mm_ss}.wav";
        var file = await FileToolkit.SaveFileAsync(".wav", fileName, AppViewModel.Instance.ActivatedWindow);
        if (file is StorageFile sf)
        {
            await ViewModel.SaveCommand.ExecuteAsync(sf.Path);
        }
    }
}

/// <summary>
/// Azure 文本转语音面板基类.
/// </summary>
public abstract class AzureTextToSpeechPanelBase : ReactiveUserControl<AzureTextToSpeechViewModel>
{
}
