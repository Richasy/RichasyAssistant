// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// Azure 语音识别视图模型.
/// </summary>
public sealed partial class AzureSpeechRecognizeViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSpeechRecognizeViewModel"/> class.
    /// </summary>
    public AzureSpeechRecognizeViewModel(AzureSpeechKernel kernel)
    {
        _kernel = kernel;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        SupportCultures = new ObservableCollection<Metadata>();
        _cacheTextList = new List<string>();
        IsContinuous = SettingsToolkit.ReadLocalSetting(SettingNames.AzureContinuousSpeechRecognize, false);
        _kernel.SpeechRecognizing += OnSpeechRecognizing;
        _kernel.SpeechRecognized += OnSpeechRecognized;
        _kernel.RecognizeStopped += OnRecognizeStopped;
    }

    [RelayCommand]
    private void RevokeEvents()
    {
        _kernel.SpeechRecognizing -= OnSpeechRecognizing;
        _kernel.SpeechRecognized -= OnSpeechRecognized;
        _kernel.RecognizeStopped -= OnRecognizeStopped;
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        IsRecording = true;
        Text = string.Empty;
        _cacheTextList.Clear();
        if (IsContinuous)
        {
            await _kernel.StartRecognizingAsync(SelectedCulture?.Id);
        }
        else
        {
            try
            {
                Text = await _kernel.RecognizeOnceAsync(SelectedCulture?.Id);
                IsRecording = false;
            }
            catch (Exception)
            {
                AppViewModel.Instance.ShowTip(StringNames.SpeechRecognizeFailed, InfoType.Error);
            }
        }
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        IsRecording = false;
        await _kernel.StopRecognizeAsync();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (!_kernel.HasValidConfig || SupportCultures.Count > 0)
        {
            return;
        }

        var voices = await _kernel.GetVoicesAsync();
        if (voices == null)
        {
            return;
        }

        var allCultures = voices
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

        var localLocale = new Metadata { Id = CultureInfo.CurrentCulture.Name, Value = CultureInfo.CurrentCulture.DisplayName };
        SelectedCulture = allCultures.Contains(localLocale)
            ? localLocale
            : SupportCultures.FirstOrDefault();
    }

    private void OnSpeechRecognizing(object sender, string e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            FormatText(e);
        });
    }

    private void OnSpeechRecognized(object sender, string e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            _cacheTextList.Add(e);
            FormatText(string.Empty);
        });
    }

    private void OnRecognizeStopped(object sender, EventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsRecording = false;
        });
    }

    private void FormatText(string text)
    {
        var t = string.Join("\n", _cacheTextList);
        Text = string.IsNullOrEmpty(t) ? text : t + "\n" + text;
    }

    partial void OnIsContinuousChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AzureContinuousSpeechRecognize, value);
}
