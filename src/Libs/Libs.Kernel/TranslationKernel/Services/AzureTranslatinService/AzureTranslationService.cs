// Copyright (c) Richasy Assistant. All rights reserved.

using Azure;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel.Translation.Services;

/// <summary>
/// Azure 翻译服务.
/// </summary>
internal sealed partial class AzureTranslationService : ITranslationService
{
    ~AzureTranslationService()
    {
        Dispose(disposing: false);
    }

    public void Initialize()
    {
        if (_client != null || !IsConfigValid())
        {
            return;
        }

        var translateKey = GlobalSettings.TryGet<string>(SettingNames.AzureTranslateKey);
        var region = GlobalSettings.TryGet<string>(SettingNames.AzureTranslateRegion);
        _client = new Azure.AI.Translation.Text.TextTranslationClient(new AzureKeyCredential(translateKey), region);
    }

    public async Task<List<Metadata>> GetSupportLanguagesAsync()
    {
        var list = TranslationDataService.GetLanguageList(TranslateType.Azure);
        if (list == null)
        {
            var data = await GetLanguagesFromOnlineAsync();
            var languageList = new Models.App.Translate.LanguageList
            {
                Id = TranslationDataService.AzureIdentify,
                Langauges = data,
            };

            await TranslationDataService.AddOrUpdateLanguageListAsync(languageList);
            list = TranslationDataService.GetLanguageList(TranslateType.Azure);
        }

        return list;
    }

    public async Task<string> TranslateTextAsync(string input, string sourceLanguageId, string targetLanguageId, CancellationToken cancellationToken)
    {
        var response = await _client.TranslateAsync(targetLanguageId, input, sourceLanguageId, cancellationToken);
        var translations = response.Value;
        var content = translations.FirstOrDefault()?.Translations?.FirstOrDefault().Text;
        return content ?? string.Empty;
    }

    public bool IsConfigValid()
    {
        var translateKey = GlobalSettings.TryGet<string>(SettingNames.AzureTranslateKey);
        var region = GlobalSettings.TryGet<string>(SettingNames.AzureTranslateRegion);

        return !string.IsNullOrEmpty(translateKey) && !string.IsNullOrEmpty(region);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
