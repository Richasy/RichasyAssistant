// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using System.Threading;
using RichasyAssistant.Libs.Translate;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 翻译视图模型.
/// </summary>
public sealed partial class TranslationViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationViewModel"/> class.
    /// </summary>
    public TranslationViewModel()
    {
        SourceLanguages = new ObservableCollection<Metadata>();
        TargetLanguages = new ObservableCollection<Metadata>();

        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand, ReloadCommand);
        AttachIsRunningToAsyncCommand(p => IsTranslating = p, TranslateCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        if (AppViewModel.Instance.TranslateClient == null)
        {
            AppViewModel.Instance.TranslateClient = new TranslateClient();
        }

        await AppViewModel.Instance.TranslateClient.InitializeAsync();
        IsAvailable = AppViewModel.Instance.TranslateClient.IsConfigValid;
        await LoadLanguagesAsync();
        IsInitialized = true;
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (!IsInitialized)
        {
            return;
        }

        await AppViewModel.Instance.TranslateClient.InitializeAsync();
        await LoadLanguagesAsync();
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
            var content = await AppViewModel.Instance.TranslateClient.TranslateTextAsync(
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
        if (_cancellationTokenSource != null
                && _cancellationTokenSource.Token.CanBeCanceled)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    private async Task LoadLanguagesAsync()
    {
        var languages = await AppViewModel.Instance.TranslateClient.GetLanguagesAsync();
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
}
