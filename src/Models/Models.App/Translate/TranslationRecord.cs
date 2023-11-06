// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace RichasyAssistant.Models.App.Translate;

/// <summary>
/// 翻译记录.
/// </summary>
public sealed class TranslationRecord
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationRecord"/> class.
    /// </summary>
    public TranslationRecord()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationRecord"/> class.
    /// </summary>
    public TranslationRecord(string sourceText, string outputText, string sourceLanguage, string targetLanguage)
    {
        SourceText = sourceText;
        OutputText = outputText;
        SourceLanguage = sourceLanguage;
        TargetLanguage = targetLanguage;
        Time = DateTimeOffset.Now;
        Id = Time.ToUnixTimeMilliseconds().ToString();
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 源文本.
    /// </summary>
    public string SourceText { get; set; }

    /// <summary>
    /// 输出文本.
    /// </summary>
    public string OutputText { get; set; }

    /// <summary>
    /// 源语言.
    /// </summary>
    public string SourceLanguage { get; set; }

    /// <summary>
    /// 目标语言.
    /// </summary>
    public string TargetLanguage { get; set; }

    /// <summary>
    /// 翻译时间.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is TranslationRecord record && Id == record.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
