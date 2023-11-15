// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Input;
using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Views;
using Windows.UI.Core;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 绘图页面.
/// </summary>
public sealed partial class DrawPage : DrawPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawPage"/> class.
    /// </summary>
    public DrawPage()
    {
        InitializeComponent();
        ViewModel = new DrawPageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
        SizeComboBox.SelectedIndex = (int)ViewModel.Size;
    }

    private void OnSizeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.Size = (OpenAIImageSize)SizeComboBox.SelectedIndex;
    }

    private void OnInputBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down
                || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.DrawCommand.Execute(default);
            }
        }
    }
}

/// <summary>
/// 绘图页面基类.
/// </summary>
public abstract class DrawPageBase : PageBase<DrawPageViewModel>
{
}
