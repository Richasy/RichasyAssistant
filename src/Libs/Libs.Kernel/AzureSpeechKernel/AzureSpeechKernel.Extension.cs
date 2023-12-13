// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NLog;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// Azure 语音服务内核.
/// </summary>
public sealed partial class AzureSpeechKernel
{
    private const string VoiceFileName = "azure_voices.json";
    private readonly Logger _logger;
    private SpeechConfig _speechConfig;
    private bool _hasValidConfig;
    private SpeechRecognizer _speechRecognizer;

    /// <summary>
    /// 是否需要依赖项.
    /// </summary>
    public bool NeedDependencies { get; set; }

    private void CheckConfig()
    {
        var voiceKey = GlobalSettings.TryGet<string>(SettingNames.AzureSpeechKey);
        var region = GlobalSettings.TryGet<string>(SettingNames.AzureSpeechRegion);

        _hasValidConfig = !string.IsNullOrEmpty(voiceKey) && !string.IsNullOrEmpty(region);

        if (_hasValidConfig && _speechConfig == null)
        {
            try
            {
                _speechConfig = SpeechConfig.FromSubscription(voiceKey, region);
                NeedDependencies = false;
            }
            catch (DllNotFoundException)
            {
                // Users need to be guided to download C++ dependencies.
                NeedDependencies = true;
                _hasValidConfig = false;
            }
        }
    }

    private async Task<List<VoiceInfo>> GetVoicesFromOnlineAsync()
    {
        using var speech = new SpeechSynthesizer(_speechConfig);
        using var result = await speech.GetVoicesAsync();
        var data = result?.Voices?.ToList();
        return data;
    }

    private void InitializeSpeechRecognizer(string locale)
    {
        CheckConfig();
        if (!string.IsNullOrEmpty(locale))
        {
            _speechConfig.SpeechRecognitionLanguage = locale;
        }

        var autoDetectSourceLanguageConfig = AutoDetectSourceLanguageConfig.FromLanguages(
                new string[] { "en-US", "zh-CN" });
        var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        _speechRecognizer = new SpeechRecognizer(_speechConfig, autoDetectSourceLanguageConfig, audioConfig);
    }

    private void OnSpeechRecognizerRecognizing(object? sender, SpeechRecognitionEventArgs e)
    {
        var text = e.Result.Text;
        if (!string.IsNullOrEmpty(text))
        {
            SpeechRecognizing?.Invoke(this, text);
        }
    }

    private void OnSpeechSessionCanceled(object? sender, SpeechRecognitionCanceledEventArgs e)
        => RecognizeStopped?.Invoke(this, EventArgs.Empty);

    private void OnSpeechSessionStopped(object? sender, SessionEventArgs e)
        => RecognizeStopped?.Invoke(this, EventArgs.Empty);

    private void OnSpeechRecognizerRecognized(object? sender, SpeechRecognitionEventArgs e)
    {
        var text = e.Result.Text;
        if (!string.IsNullOrEmpty(text))
        {
            SpeechRecognized?.Invoke(this, text);
        }
    }
}
