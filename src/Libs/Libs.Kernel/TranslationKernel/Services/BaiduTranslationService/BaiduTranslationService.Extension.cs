// Copyright (c) Richasy Assistant. All rights reserved.

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RichasyAssistant.Libs.Locator;

namespace RichasyAssistant.Libs.Kernel.Translation.Services;

/// <summary>
/// 百度翻译服务.
/// </summary>
internal sealed partial class BaiduTranslationService
{
    private const string TranslateApi = "http://api.fanyi.baidu.com/api/trans/vip/translate";
    private const int MaxTextLength = 1100;
    private HttpClient _httpClient;
    private string _salt;
    private bool _disposedValue;

    private async Task<string> TranslateInternalAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken)
    {
        text = text.Replace("\r", "\n").Replace("\n\n", "\n");
        var appId = GlobalSettings.TryGet<string>(Models.Constants.SettingNames.BaiduTranslateAppId);
        var dict = new Dictionary<string, string>
        {
            { "q", text },
            { "from", sourceLanguage },
            { "to", targetLanguage },
            { "appid", appId },
            { "salt", _salt },
            { "sign", GenerateSign(text) },
        };

        var content = new FormUrlEncodedContent(dict);
        var request = new HttpRequestMessage(HttpMethod.Post, TranslateApi);
        request.Content = content;
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        var jele = JsonSerializer.Deserialize<JsonElement>(result);
        if (jele.TryGetProperty("error_msg", out var errorEle))
        {
            throw new Exception(errorEle.GetRawText());
        }
        else
        {
            var sb = new StringBuilder();
            var hasResult = jele.TryGetProperty("trans_result", out var transResultEle);
            if (!hasResult)
            {
                throw new Exception("No result.");
            }

            foreach (var item in transResultEle.EnumerateArray())
            {
                if (item.TryGetProperty("dst", out var dstEle))
                {
                    sb.AppendLine(dstEle.ToString());
                }
            }

            return sb.ToString().Trim();
        }
    }

    private string GenerateSign(string query)
    {
        var appId = GlobalSettings.TryGet<string>(Models.Constants.SettingNames.BaiduTranslateAppId);
        var appKey = GlobalSettings.TryGet<string>(Models.Constants.SettingNames.BaiduTranslateAppKey);
        var byteOld = Encoding.UTF8.GetBytes(appId + query + _salt + appKey);
        var byteNew = MD5.HashData(byteOld);
        var sb = new StringBuilder();
        foreach (var b in byteNew)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _httpClient = null;
            _disposedValue = true;
        }
    }
}
