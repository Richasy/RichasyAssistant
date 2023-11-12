// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Kernel.Translation.Services;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.App.Translate;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel.Translation;

/// <summary>
/// 翻译客户端.
/// </summary>
public sealed class TranslationKernel : IDisposable
{
    private TranslationKernel()
    {
    }

    /// <summary>
    /// 翻译服务.
    /// </summary>
    public ITranslationService Service { get; private set; }

    /// <summary>
    /// 配置是否有效.
    /// </summary>
    public bool IsConfigValid => Service?.IsConfigValid() ?? false;

    /// <summary>
    /// 创建实例.
    /// </summary>
    /// <returns><see cref="TranslationKernel"/>.</returns>
    public static TranslationKernel Create()
    {
        var defaultService = GlobalSettings.TryGet<TranslateType>(SettingNames.DefaultTranslate);
        var kernel = new TranslationKernel();
        if (defaultService is TranslateType.Azure)
        {
            kernel.Service = new AzureTranslationService();
        }
        else if (defaultService is TranslateType.Baidu)
        {
            kernel.Service = new BaiduTranslationService();
        }

        kernel.Service.Initialize();
        return kernel;
    }

    /// <summary>
    /// 获取当前语言列表.
    /// </summary>
    /// <returns>语言列表.</returns>
    public async Task<List<Metadata>> GetLanguagesAsync()
    {
        if (Service is null)
        {
            throw new KernelException(KernelExceptionType.TranslationServiceNotInitialized);
        }

        var languages = await Service.GetSupportLanguagesAsync();
        return languages.OrderBy(p => p.Value).ToList();
    }

    /// <summary>
    /// 翻译文本.
    /// </summary>
    /// <returns>翻译结果.</returns>
    public async Task<string> TranslateTextAsync(string input, string sourceLanguageId, string targetLanguageId, CancellationToken cancellationToken)
    {
        if (Service is null)
        {
            throw new KernelException(KernelExceptionType.TranslationServiceNotInitialized);
        }

        var text = await Service.TranslateTextAsync(input, sourceLanguageId, targetLanguageId, cancellationToken);

        try
        {
            var record = new TranslationRecord(input, text, sourceLanguageId, targetLanguageId);
            await TranslationDataService.AddRecordAsync(record);
        }
        catch (Exception)
        {
        }

        return text;
    }

    /// <inheritdoc/>
    public void Dispose()
        => Service?.Dispose();
}
