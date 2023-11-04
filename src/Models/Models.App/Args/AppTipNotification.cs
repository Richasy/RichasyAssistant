// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.Args;

/// <summary>
/// 应用提示通知.
/// </summary>
public class AppTipNotification : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppTipNotification"/> class.
    /// </summary>
    /// <param name="msg">消息内容.</param>
    /// <param name="type">提示类型.</param>
    public AppTipNotification(string msg, InfoType type = InfoType.Information, object targetWindow = default)
    {
        Message = msg;
        Type = type;
        TargetWindow = targetWindow;
    }

    /// <summary>
    /// 消息内容.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 提示类型.
    /// </summary>
    public InfoType Type { get; set; }

    /// <summary>
    /// 目标窗口.
    /// </summary>
    public object TargetWindow { get; set; }
}
