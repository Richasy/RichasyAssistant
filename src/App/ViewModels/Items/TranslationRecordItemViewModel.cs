// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using Humanizer;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Translate;
using Windows.ApplicationModel.DataTransfer;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 翻译记录项视图模型.
/// </summary>
public sealed partial class TranslationRecordItemViewModel : DataViewModelBase<TranslationRecord>
{
    [ObservableProperty]
    private string _sourceLanguage;

    [ObservableProperty]
    private string _targetLanguage;

    [ObservableProperty]
    private string _time;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationRecordItemViewModel"/> class.
    /// </summary>
    public TranslationRecordItemViewModel(TranslationRecord data)
        : base(data)
    {
        if (string.IsNullOrEmpty(data.SourceLanguage))
        {
            SourceLanguage = ResourceToolkit.GetLocalizedString(StringNames.AutoDetect);
        }
        else
        {
            var sourceLan = new CultureInfo(data.SourceLanguage);
            SourceLanguage = sourceLan.DisplayName;
        }

        var targetLan = new CultureInfo(data.TargetLanguage);
        TargetLanguage = targetLan.DisplayName;

        Time = data.Time.Humanize();
    }

    private static void Copy(string text)
    {
        var dp = new DataPackage();
        dp.SetText(text);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(StringNames.Copied, InfoType.Success);
    }

    [RelayCommand]
    private void CopySource()
        => Copy(Data.SourceText);

    [RelayCommand]
    private void CopyOutput()
        => Copy(Data.OutputText);
}
