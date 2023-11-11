// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Args;

/// <summary>
/// 应用导航事件参数.
/// </summary>
public sealed class AppNavigateEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppNavigateEventArgs"/> class.
    /// </summary>
    public AppNavigateEventArgs(Type type, object parameter)
    {
        PageType = type;
        Parameter = parameter;
    }

    /// <summary>
    /// 页面类型.
    /// </summary>
    public Type PageType { get; set; }

    /// <summary>
    /// 导航参数.
    /// </summary>
    public object Parameter { get; set; }
}
