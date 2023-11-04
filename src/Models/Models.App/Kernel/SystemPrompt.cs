// Copyright (c) Reader Copilot. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 系统提示词.
/// </summary>
public sealed class SystemPrompt
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SystemPrompt"/> class.
    /// </summary>
    public SystemPrompt()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemPrompt"/> class.
    /// </summary>
    public SystemPrompt(string name, string prompt)
    {
        Id = Guid.NewGuid().ToString("N");
        Name = name;
        Prompt = prompt;
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 显示名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 提示内容.
    /// </summary>
    public string Prompt { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SystemPrompt prompt && Id == prompt.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
