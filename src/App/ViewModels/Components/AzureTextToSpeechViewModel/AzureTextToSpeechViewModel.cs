// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Models.App.Kernel;
using Windows.Media.Playback;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// Azure 语音转文本视图模型.
/// </summary>
public sealed partial class AzureTextToSpeechViewModel : ViewModelBase, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTextToSpeechViewModel"/> class.
    /// </summary>
    public AzureTextToSpeechViewModel(AzureSpeechKernel kernel)
    {
        _kernel = kernel;
        _allVoices = new List<AzureSpeechVoice>();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _player = new MediaPlayer();
        _player.CurrentStateChanged += OnMediaPlayerStateChanged;
        IsPaused = true;
        SupportCultures = new ObservableCollection<Metadata>();
        DisplayVoices = new ObservableCollection<AzureSpeechVoice>();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _player?.Dispose();
        _speechStream?.Dispose();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (!_kernel.HasValidConfig || _allVoices.Count != 0)
        {
            return;
        }

        await ReloadMetadataAsync();
    }

    [RelayCommand]
    private async Task ReloadMetadataAsync()
    {
        TryClear(SupportCultures);
        try
        {
            _allVoices.Clear();
            var voices = await _kernel.GetVoicesAsync();
            foreach (var vo in voices)
            {
                _allVoices.Add(vo);
            }

            var allCultures = _allVoices
                .Select(p => p.Locale)
                .Distinct()
                .Select(p =>
                {
                    var culture = new CultureInfo(p);
                    return new Metadata { Id = culture.Name, Value = culture.DisplayName };
                })
                .OrderBy(p => p.Value)
                .ToList();
            foreach (var item in allCultures)
            {
                SupportCultures.Add(item);
            }

            var localLanguage = SettingsToolkit.ReadLocalSetting(SettingNames.AzureTextToSpeechLanguage, string.Empty);
            var localCulture = string.IsNullOrEmpty(localLanguage)
                ? CultureInfo.CurrentCulture
                : new CultureInfo(localLanguage);
            var localLocale = new Metadata { Id = localCulture.Name, Value = localCulture.DisplayName };
            var culture = allCultures.Contains(localLocale)
                ? localLocale
                : SupportCultures.FirstOrDefault();

            SelectedCulture = culture;
        }
        catch (Exception)
        {
            AppViewModel.Instance.ShowTip(StringNames.RequestVoiceMetadataFailed, InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task ReadAsync()
    {
        if (IsConverting)
        {
            return;
        }

        _speechStream = default;
        if (string.IsNullOrEmpty(Text))
        {
            AppViewModel.Instance.ShowTip(StringNames.NeedInputText, InfoType.Error);
            return;
        }

        if (SelectedVoice == null)
        {
            AppViewModel.Instance.ShowTip(StringNames.NeedSelectVoice, InfoType.Error);
            return;
        }

        try
        {
            _speechStream?.Dispose();
            _speechStream = null;
            IsConverting = true;
            IsAudioEnabled = false;
            var stream = await _kernel.GetSpeechAsync(Text, SelectedVoice.Id);
            if (stream == null)
            {
                AppViewModel.Instance.ShowTip(StringNames.SpeechConvertFailed, InfoType.Error);
                return;
            }

            IsAudioEnabled = true;
            _speechStream = new MemoryStream();
            await stream.CopyToAsync(_speechStream);
            _player.SetStreamSource(stream.AsRandomAccessStream());
            _player.Play();
        }
        catch (TaskCanceledException)
        {
            // Do nothing.
        }
        catch (Exception)
        {
            AppViewModel.Instance.ShowTip(StringNames.SpeechConvertFailed, InfoType.Error);
        }

        IsConverting = false;
    }

    [RelayCommand]
    private async Task SaveAsync(string filePath)
    {
        if (_speechStream == null || string.IsNullOrEmpty(filePath))
        {
            return;
        }

        _speechStream.Seek(0, SeekOrigin.Begin);
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await _speechStream.CopyToAsync(fileStream);
        AppViewModel.Instance.ShowTip(StringNames.FileSaved, InfoType.Success);
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (!_player.CanSeek)
        {
            return;
        }

        if (_player.CurrentState == MediaPlayerState.Playing)
        {
            _player.Pause();
        }
        else
        {
            _player.Play();
        }
    }

    [RelayCommand]
    private void ResetVoice()
    {
        TryClear(DisplayVoices);
        var voices = _allVoices
             .Where(p => p.Locale.Equals(SelectedCulture.Id, StringComparison.InvariantCultureIgnoreCase))
             .OrderBy(p => p.IsFemale)
             .ToList();
        foreach (var item in voices)
        {
            DisplayVoices.Add(item);
        }

        var localVoiceId = SettingsToolkit.ReadLocalSetting(SettingNames.AzureTextToSpeechVoice, string.Empty);
        var voice = string.IsNullOrEmpty(localVoiceId)
            ? DisplayVoices.FirstOrDefault()
            : DisplayVoices.FirstOrDefault(p => p.Id == localVoiceId) ?? DisplayVoices.FirstOrDefault();

        SelectedVoice = voice;
    }

    private void OnMediaPlayerStateChanged(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsPaused = sender.CurrentState != MediaPlayerState.Playing;
        });
    }

    partial void OnSelectedCultureChanged(Metadata value)
    {
        if (value == null)
        {
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.AzureTextToSpeechLanguage, value.Id);
        ResetVoiceCommand.Execute(default);
    }

    partial void OnSelectedVoiceChanged(AzureSpeechVoice value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AzureTextToSpeechVoice, value?.Id ?? string.Empty);
}
