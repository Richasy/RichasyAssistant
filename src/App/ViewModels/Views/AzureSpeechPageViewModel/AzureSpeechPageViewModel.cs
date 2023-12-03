// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Libs.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// Azure 语音服务视图模型.
/// </summary>
public sealed partial class AzureSpeechPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSpeechPageViewModel"/> class.
    /// </summary>
    public AzureSpeechPageViewModel()
    {
        _kernel = new AzureSpeechKernel();
        TextToSpeech = new AzureTextToSpeechViewModel(_kernel);
        SpeechRecognition = new AzureSpeechRecognizeViewModel(_kernel);

        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        _kernel.ReloadConfig();
        await TextToSpeech.InitializeCommand.ExecuteAsync(default);
        await SpeechRecognition.InitializeCommand.ExecuteAsync(default);
        IsConfigInvalid = !_kernel.HasValidConfig;
        NeedDependencies = _kernel.NeedDependencies;
        ErrorDescription = NeedDependencies
            ? ResourceToolkit.GetLocalizedString(StringNames.VoiceDependenciesMissing)
            : ResourceToolkit.GetLocalizedString(StringNames.VoiceConfigInvalid);
    }
}
