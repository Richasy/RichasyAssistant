// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.App.Translate;
using RichasyAssistant.Models.Context;

namespace RichasyAssistant.Libs.Service;

/// <summary>
/// 翻译数据服务.
/// </summary>
public static partial class TranslationDataService
{
    /// <summary>
    /// Azure 识别标识.
    /// </summary>
    public const string AzureIdentify = "azure";

    /// <summary>
    /// 百度识别标识.
    /// </summary>
    public const string BaiduIdentify = "baidu";

    private const string DbName = "trans.db";

    private static readonly Dictionary<string, List<Metadata>> _languages = new();
    private static TranslationDbContext _dbContext;
    private static List<TranslationRecord> _history;
}
