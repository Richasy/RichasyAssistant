// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using System.Threading;
using RichasyAssistant.Libs.Kernel.Translation;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;
using Windows.ApplicationModel.DataTransfer;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 翻译视图模型.
/// </summary>
public sealed partial class TranslationPageViewModel : ViewModelBase, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationPageViewModel"/> class.
    /// </summary>
    public TranslationPageViewModel()
    {
        SourceLanguages = new ObservableCollection<Metadata>();
        TargetLanguages = new ObservableCollection<Metadata>();

        SourceFontSize = 20;
        OutputFontSize = 20;

        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
        AttachIsRunningToAsyncCommand(p => IsTranslating = p, TranslateCommand);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        SourceLanguages.Clear();
        TargetLanguages.Clear();
        Cancel();
        _kernel?.Dispose();
        _kernel = default;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_kernel != null)
        {
            return;
        }

        var serviceName = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultTranslate, TranslateType.Azure) == TranslateType.Azure
            ? ResourceToolkit.GetLocalizedString(StringNames.AzureTranslate)
            : ResourceToolkit.GetLocalizedString(StringNames.BaiduTranslate);
        PoweredBy = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PoweredBy), serviceName);

        await TranslationDataService.InitializeAsync();
        _kernel = TranslationKernel.Create();
        IsAvailable = _kernel.IsConfigValid;
        await LoadLanguagesAsync();
        IsInitialized = true;
    }

    [RelayCommand]
    private async Task TranslateAsync()
    {
        OutputText = string.Empty;
        ErrorText = string.Empty;
        if (string.IsNullOrEmpty(SourceText)
            || SourceLanguage == default
            || TargetLanguage == default)
        {
            return;
        }

        try
        {
            Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var content = await _kernel.TranslateTextAsync(
                SourceText,
                SourceLanguage.Id,
                TargetLanguage.Id,
                _cancellationTokenSource.Token);
            _cancellationTokenSource = default;
            OutputText = content;
        }
        catch (TaskCanceledException)
        {
            // 用户取消了翻译.
        }
        catch (Exception ex)
        {
            ErrorText = ex.Message;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = default;
    }

    [RelayCommand]
    private void CopyOutput()
    {
        if (string.IsNullOrEmpty(OutputText))
        {
            return;
        }

        var dp = new DataPackage();
        dp.SetText(OutputText);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(StringNames.Copied, InfoType.Success);
    }

    private async Task LoadLanguagesAsync()
    {
        var languages = await _kernel.GetLanguagesAsync();
        var localLocale = languages.FirstOrDefault(p => p.Id == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        var localSourceLanguage = SettingsToolkit.ReadLocalSetting(SettingNames.TranslationSourceLanguage, string.Empty);
        var localTargetLanguage = SettingsToolkit.ReadLocalSetting(SettingNames.TranslationTargetLanguage, localLocale?.Id ?? "en");
        foreach (var item in languages)
        {
            SourceLanguages.Add(item);
            TargetLanguages.Add(item);
        }

        SourceLanguages.Insert(0, new Metadata { Id = string.Empty, Value = ResourceToolkit.GetLocalizedString(StringNames.AutoDetect) });
        SourceLanguage = SourceLanguages.FirstOrDefault(p => p.Id == localSourceLanguage) ?? SourceLanguages[0];
        TargetLanguage = TargetLanguages.FirstOrDefault(p => p.Id == localTargetLanguage) ?? TargetLanguages.FirstOrDefault(p => p.Id.StartsWith("en"));
    }

    partial void OnSourceLanguageChanged(Metadata value)
    {
        if (value == null)
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.TranslationSourceLanguage);
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.TranslationSourceLanguage, value.Id);
    }

    partial void OnTargetLanguageChanged(Metadata value)
    {
        if (value == null)
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.TranslationTargetLanguage);
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.TranslationTargetLanguage, value.Id);
        if (!string.IsNullOrEmpty(SourceText))
        {
            TranslateCommand.Execute(default);
        }
    }

    partial void OnSourceTextChanged(string value)
    {
        SourceCharacterCount = value.Length;
        SourceFontSize = SourceCharacterCount < 200 ? 20 : 16;
    }

    partial void OnOutputTextChanged(string value)
        => OutputFontSize = value.Length < 400 ? 20 : 16;
}
