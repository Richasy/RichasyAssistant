// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using System.Threading;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel.Translation;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.App.Translate;
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
        History = new ObservableCollection<TranslationRecordItemViewModel>();

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
        LoadHistory(true);
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

            await TryAddRecordAsync();
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

    [RelayCommand]
    private void LoadHistory(bool force = false)
    {
        if (!force && !HistoryHasMore)
        {
            return;
        }

        var hasMore = TranslationDataService.HasMoreHistory(HistoryPageIndex);
        if (hasMore)
        {
            var list = TranslationDataService.GetHistory(HistoryPageIndex);
            foreach (var item in list)
            {
                History.Add(new TranslationRecordItemViewModel(item));
            }

            HistoryPageIndex++;
            HistoryHasMore = TranslationDataService.HasMoreHistory(HistoryPageIndex);
        }

        IsHistoryEmpty = History.Count == 0;
    }

    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        await TranslationDataService.ClearHistoryAsync();
        TryClear(History);
        HistoryHasMore = false;
        HistoryPageIndex = 0;
        IsHistoryEmpty = true;
    }

    [RelayCommand]
    private async Task RemoveRecordAsync(TranslationRecordItemViewModel item)
    {
        await TranslationDataService.RemoveRecordAsync(item.Data.Id);
        _ = History.Remove(item);
        IsHistoryEmpty = History.Count == 0;
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

    private async Task TryAddRecordAsync()
    {
        var shouldRecord = SettingsToolkit.ReadLocalSetting(SettingNames.RecordTranslationResult, true);
        if (!shouldRecord)
        {
            return;
        }

        var record = new TranslationRecord(SourceText, OutputText, SourceLanguage.Id, TargetLanguage.Id);
        await TranslationDataService.AddRecordAsync(record);
        if (History.Count > 100)
        {
            History.RemoveAt(History.Count - 1);
        }

        History.Insert(0, new TranslationRecordItemViewModel(record));
        IsHistoryEmpty = false;
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
