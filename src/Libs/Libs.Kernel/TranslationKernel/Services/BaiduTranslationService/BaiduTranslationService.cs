// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using System.Text;
using System.Text.Json;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel.Translation.Services;

/// <summary>
/// 百度翻译服务.
/// </summary>
internal sealed partial class BaiduTranslationService : ITranslationService
{
    public BaiduTranslationService()
        => _httpClient = new HttpClient();

    /// <summary>
    /// Finalizes an instance of the <see cref="BaiduTranslationService"/> class.
    /// </summary>
    ~BaiduTranslationService()
    {
        Dispose(disposing: false);
    }

    public void Initialize()
        => _salt = new Random().Next(100000).ToString();

    public async Task<List<Metadata>> GetSupportLanguagesAsync()
    {
        var list = TranslationDataService.GetLanguageList(TranslateType.Baidu);
        if (list == null)
        {
            var installedPath = AppDomain.CurrentDomain.BaseDirectory;
            var file = Path.Combine(installedPath, "Assets/baiduLan.json");
            var json = await File.ReadAllTextAsync(file);
            var data = JsonSerializer.Deserialize<List<string>>(json);
            var result = new List<Metadata>();
            foreach (var item in data)
            {
                var locale = new CultureInfo(item);
                result.Add(new Metadata { Id = item, Value = locale.EnglishName ?? locale.Name });
            }

            var languageList = new Models.App.Translate.LanguageList
            {
                Id = TranslationDataService.BaiduIdentify,
                Langauges = result,
            };
            await TranslationDataService.AddOrUpdateLanguageListAsync(languageList);
            list = TranslationDataService.GetLanguageList(TranslateType.Baidu);
        }

        return list;
    }

    public bool IsConfigValid()
    {
        var appId = GlobalSettings.TryGet<string>(SettingNames.BaiduTranslateAppId);
        var appKey = GlobalSettings.TryGet<string>(SettingNames.BaiduTranslateAppKey);
        return !string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(appKey);
    }

    public async Task<string> TranslateTextAsync(string input, string sourceLanguageId, string targetLanguageId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(sourceLanguageId))
        {
            sourceLanguageId = "auto";
        }

        input = input.Replace("\r", "\n").Replace("\n\n", "\n");
        var textBuilder = new StringBuilder();
        var resultBuilder = new StringBuilder();
        foreach (var item in input.Split('\n'))
        {
            if (textBuilder.Length + item.Length > MaxTextLength)
            {
                // Due to the API request rate limit,
                // a translation is requested at most once per second,
                // so additional blocking is required here
                if (resultBuilder.Length > 0)
                {
                    await Task.Delay(1000, cancellationToken);
                }

                var tempResult = await TranslateInternalAsync(textBuilder.ToString(), sourceLanguageId, targetLanguageId, cancellationToken);
                resultBuilder.AppendLine(tempResult);
                textBuilder.Clear();
            }

            textBuilder.AppendLine(item);
        }

        if (textBuilder.Length > 0)
        {
            if (resultBuilder.Length > 0)
            {
                await Task.Delay(1000, cancellationToken);
            }

            var tempResult = await TranslateInternalAsync(textBuilder.ToString(), sourceLanguageId, targetLanguageId, cancellationToken);
            resultBuilder.AppendLine(tempResult);
        }

        return resultBuilder.ToString();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
