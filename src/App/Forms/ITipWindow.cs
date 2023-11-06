// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.Forms;

/// <summary>
/// 可以显示提示的窗口.
/// </summary>
public interface ITipWindow
{
    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="element">提示元素.</param>
    /// <param name="duration">显示几秒.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task ShowTipAsync(UIElement element, double duration);
}
