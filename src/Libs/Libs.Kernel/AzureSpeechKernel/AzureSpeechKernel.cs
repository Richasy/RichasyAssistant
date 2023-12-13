// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using Microsoft.CognitiveServices.Speech;
using NLog;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// Azure 语音服务内核.
/// </summary>
public sealed partial class AzureSpeechKernel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSpeechKernel"/> class.
    /// </summary>
    public AzureSpeechKernel()
        => _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Occurs when speech recognizing.
    /// </summary>
    public event EventHandler<string> SpeechRecognizing;

    /// <summary>
    /// Occurs when speech recognized.
    /// </summary>
    public event EventHandler<string> SpeechRecognized;

    /// <summary>
    /// Occurs when speech recognition stopped.
    /// </summary>
    public event EventHandler RecognizeStopped;

    /// <summary>
    /// 配置是否有效.
    /// </summary>
    public bool HasValidConfig => _hasValidConfig;

    /// <summary>
    /// Read input text.
    /// </summary>
    /// <param name="text">Text to be read aloud.</param>
    /// <param name="voiceId">Voice id.</param>
    /// <returns>Speech stream.</returns>
    public async Task<Stream> GetSpeechAsync(string text, string voiceId)
    {
        CheckConfig();
        _speechConfig.SpeechSynthesisVoiceName = voiceId;
        using var speech = new SpeechSynthesizer(_speechConfig, default);
        using var result = await speech.SpeakTextAsync(text);
        switch (result.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                var ms = new MemoryStream(result.AudioData);
                return ms;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                if (cancellation.Reason == CancellationReason.Error)
                {
                    _logger.Error($"Speech cancelled: ${cancellation.ErrorCode}, ${cancellation.ErrorDetails}");
                    throw new InvalidCastException("Something wrong");
                }
                else if (cancellation.Reason == CancellationReason.CancelledByUser)
                {
                    throw new TaskCanceledException();
                }

                break;
            default:
                _logger.Error($"Speech not completed: ${result.Reason}");
                break;
        }

        return default;
    }

    /// <summary>
    /// Get all voices.
    /// </summary>
    /// <returns>Voice list.</returns>
    public async Task<List<AzureSpeechVoice>> GetVoicesAsync()
    {
        var localFolderPath = GlobalSettings.TryGet<string>(Models.Constants.SettingNames.LocalFolderPath);
        var voiceFilePath = Path.Combine(localFolderPath, VoiceFileName);

        var voices = new List<AzureSpeechVoice>();
        if (File.Exists(voiceFilePath))
        {
            var content = await File.ReadAllTextAsync(voiceFilePath);
            try
            {
                voices = JsonSerializer.Deserialize<List<AzureSpeechVoice>>(content);
            }
            catch (Exception)
            {
            }
        }

        if (voices == null || voices.Count == 0)
        {
            var onlineVoices = await GetVoicesFromOnlineAsync();
            if (onlineVoices == null)
            {
                return default;
            }

            voices = onlineVoices.Select(p => new AzureSpeechVoice
                {
                    Id = p.ShortName,
                    Name = p.LocalName,
                    IsFemale = p.Gender == SynthesisVoiceGender.Female,
                    IsNeural = p.VoiceType.ToString().Contains("Neural"),
                    Locale = p.Locale,
                })
                .OrderBy(p => p.Id)
                .ToList();

            var jsonText = JsonSerializer.Serialize(voices);
            await File.WriteAllTextAsync(voiceFilePath, jsonText);
        }

        return voices;
    }

    /// <summary>
    /// Do a short speech recognition.
    /// </summary>
    /// <returns>Recognized content.</returns>
    public async Task<string> RecognizeOnceAsync(string locale)
    {
        InitializeSpeechRecognizer(locale);
        var result = await _speechRecognizer.RecognizeOnceAsync();
        if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            if (cancellation.Reason == CancellationReason.Error)
            {
                _logger.Error($"Speech recognition interrupted: {cancellation.ErrorCode}, {cancellation.ErrorDetails}");
                throw new System.Exception("Recognize failed.");
            }
        }

        return result.Text ?? string.Empty;
    }

    /// <summary>
    /// Start continuous speech recognition.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task StartRecognizingAsync(string locale)
    {
        InitializeSpeechRecognizer(locale);
        _speechRecognizer.Recognizing += OnSpeechRecognizerRecognizing;
        _speechRecognizer.Recognized += OnSpeechRecognizerRecognized;
        _speechRecognizer.SessionStopped += OnSpeechSessionStopped;
        _speechRecognizer.Canceled += OnSpeechSessionCanceled;
        _logger.Info("Speech recognition activated");
        await _speechRecognizer.StartContinuousRecognitionAsync();
        _logger.Info("Speech recognition is off");
    }

    /// <summary>
    /// End continuous speech recognition and return the recognized text.
    /// </summary>
    /// <returns>Recognized content.</returns>
    public Task StopRecognizeAsync()
        => _speechRecognizer.StopContinuousRecognitionAsync();

    /// <summary>
    /// Reload configuration.
    /// </summary>
    public void ReloadConfig()
    {
        _speechConfig = default;
        CheckConfig();
    }
}
