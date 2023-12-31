﻿// Copyright (c) Richasy Assistant. All rights reserved.

using Azure.AI.Translation.Text;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.Libs.Kernel.Translation.Services;

/// <summary>
/// Azure 翻译服务.
/// </summary>
internal sealed partial class AzureTranslationService
{
    private bool _disposedValue;
    private TextTranslationClient _client;

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
                data.Add(new Metadata { Id = id, Value = name });
            }
        }

        return data;
    }
}
