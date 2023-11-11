// Copyright (c) Richasy Assistant. All rights reserved.

using Azure.AI.Translation.Text;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Context;

namespace RichasyAssistant.Libs.Translate.Services;

/// <summary>
/// Azure 翻译服务.
/// </summary>
internal sealed partial class AzureTranslateService
{
    private bool _disposedValue;
    private TextTranslationClient _client;

    private static TranslationDbContext GetDbContext()
    {
        return GlobalVariables.TryGet<TranslationDbContext>(Models.Constants.VariableNames.TranslationDbContext)
            ?? throw new KernelException(KernelExceptionType.TranslationDbNotInitialized);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _client = null;
            }

            _disposedValue = true;
        }
    }

    private async Task<List<Metadata>> GetLanguagesFromOnlineAsync()
    {
        var languages = await _client.GetLanguagesAsync();
        var translation = languages?.Value?.Translation;
        var data = new List<Metadata>();
        if (translation != null)
        {
            foreach (var item in translation)
            {
                var id = item.Key;
                var name = item.Value.Name;
                data.Add(new Metadata { Id = id + "_a", Value = name });
            }
        }

        return data;
    }
}
