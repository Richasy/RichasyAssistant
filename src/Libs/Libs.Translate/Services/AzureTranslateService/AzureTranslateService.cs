// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using Azure;
using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Translate.Services;

/// <summary>
/// Azure 翻译服务.
/// </summary>
internal sealed partial class AzureTranslateService : ITranslateService
{
    ~AzureTranslateService()
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
        var context = GetDbContext();
        var list = context.Languages.AsNoTracking().Include(p => p.Langauges).FirstOrDefault(p => p.Id == "azure")?.Langauges;
        if (list == null)
        {
            var data = await GetLanguagesFromOnlineAsync();
            await context.Languages.AddAsync(new Models.App.Translate.LanguageList
            {
                Id = "azure",
                Langauges = data,
            });

            await context.SaveChangesAsync();
            list = data;
        }

        foreach (var item in list)
        {
            item.Id = item.Id.Replace("_a", string.Empty);
            var culture = new CultureInfo(item.Id);
            item.Value = culture.DisplayName;
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
