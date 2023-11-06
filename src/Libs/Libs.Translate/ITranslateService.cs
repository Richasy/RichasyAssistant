// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.Libs.Translate;

/// <summary>
/// 翻译客户端的接口定义.
/// </summary>
internal interface ITranslateService : IDisposable
{
    /// <summary>
    /// 获取受支持的语言列表.
    /// </summary>
    /// <returns>元数据.</returns>
    Task<List<Metadata>> GetSupportLanguagesAsync();

    /// <summary>
    /// 翻译文本.
    /// </summary>
    /// <param name="input">文本输入.</param>
    /// <param name="sourceLanguageId">原始文本的语言.</param>
    /// <param name="targetLanguageId">翻译文本的语言.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>翻译后的文本.</returns>
    Task<string> TranslateTextAsync(string input, string sourceLanguageId, string targetLanguageId, CancellationToken cancellationToken);

    /// <summary>
    /// 配置是否有效.
    /// </summary>
    /// <returns>是否有效.</returns>
    bool IsConfigValid();

    /// <summary>
    /// 初始化.
    /// </summary>
    void Initialize();
}
