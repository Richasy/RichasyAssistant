// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Libs.Translate.Services;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.App.Translate;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Models.Context;

namespace RichasyAssistant.Libs.Translate;

/// <summary>
/// 翻译客户端.
/// </summary>
public sealed class TranslateClient
{
    private ITranslateService _service;

    /// <summary>
    /// 配置是否有效.
    /// </summary>
    public bool IsConfigValid => _service?.IsConfigValid() ?? false;

    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InitializeAsync()
    {
        var defaultService = GlobalSettings.TryGet<TranslateType>(SettingNames.DefaultTranslate);
        if (defaultService is TranslateType.Azure && _service is not AzureTranslateService)
        {
            _service?.Dispose();
            _service = new AzureTranslateService();
        }
        else if (defaultService is TranslateType.Baidu && _service is not BaiduTranslateService)
        {
            _service?.Dispose();
            _service = new BaiduTranslateService();
        }

        _service.Initialize();
        await InitializeLocalDatabaseAsync();
    }

    /// <summary>
    /// 获取当前语言列表.
    /// </summary>
    /// <returns>语言列表.</returns>
    public async Task<List<Metadata>> GetLanguagesAsync()
    {
        if (_service is null)
        {
            throw new KernelException(KernelExceptionType.TranslationServiceNotInitialized);
        }

        var languages = await _service.GetSupportLanguagesAsync();
        return languages.OrderBy(p => p.Value).ToList();
    }

    /// <summary>
    /// 翻译文本.
    /// </summary>
    /// <returns>翻译结果.</returns>
    public async Task<string> TranslateTextAsync(string input, string sourceLanguageId, string targetLanguageId, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new KernelException(KernelExceptionType.TranslationServiceNotInitialized);
        }

        var text = await _service.TranslateTextAsync(input, sourceLanguageId, targetLanguageId, cancellationToken);

        try
        {
            var dbContext = GlobalVariables.TryGet<TranslationDbContext>(VariableNames.TranslationDbContext);
            var record = new TranslationRecord(input, text, sourceLanguageId, targetLanguageId);
            await dbContext.History.AddAsync(record, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
        }

        return text;
    }

    private static async Task InitializeLocalDatabaseAsync()
    {
        if (GlobalVariables.TryGet<TranslationDbContext>(VariableNames.TranslationDbContext) != null)
        {
            return;
        }

        var libPath = GlobalSettings.TryGet<string>(SettingNames.LibraryFolderPath);
        if (string.IsNullOrEmpty(libPath))
        {
            throw new KernelException(KernelExceptionType.LibraryNotInitialized);
        }

        var localDbPath = Path.Combine(libPath, "trans.db");
        if (!File.Exists(localDbPath))
        {
            var defaultTransDbPath = GlobalSettings.TryGet<string>(SettingNames.DefaultTranslationDbPath);
            if (string.IsNullOrEmpty(defaultTransDbPath)
                || !File.Exists(defaultTransDbPath))
            {
                throw new KernelException(KernelExceptionType.TranslationDbNotInitialized);
            }

            await Task.Run(() =>
            {
                File.Copy(defaultTransDbPath, localDbPath, true);
            });
        }

        var context = new TranslationDbContext(localDbPath);
        GlobalVariables.Set(VariableNames.TranslationDbContext, context);
    }
}
