// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel.DataAnnotations;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 助理.
/// </summary>
public sealed class Assistant
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Assistant"/> class.
    /// </summary>
    public Assistant()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Assistant"/> class.
    /// </summary>
    public Assistant(string name, string desc, string instruction)
    {
        Id = Guid.NewGuid().ToString("N");
        Name = name;
        Description = desc;
        Instruction = instruction;
        UseDefaultKernel = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Assistant"/> class.
    /// </summary>
    public Assistant(string name, string desc, string instruction, KernelType kernel, string modelName, string modelDeployName = null)
        : this(name, desc, instruction)
    {
        UseDefaultKernel = false;
        Kernel = kernel;
        Model = modelName;
        Remark = modelDeployName ?? string.Empty;
    }

    /// <summary>
    /// 助理标识符.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 助理名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 助理描述.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 内核类型.
    /// </summary>
    public KernelType Kernel { get; set; }

    /// <summary>
    /// 模型名称.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 指令（系统提示词）.
    /// </summary>
    public string? Instruction { get; set; }

    /// <summary>
    /// 备注信息.
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 是否使用默认内核.
    /// </summary>
    public bool UseDefaultKernel { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Assistant assistant && Id == assistant.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
