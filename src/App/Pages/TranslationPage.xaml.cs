// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using Microsoft.UI.Input;
using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Components;
using Windows.UI.Core;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 翻译页面.
/// </summary>
public sealed partial class TranslationPage : TranslationPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationPage"/> class.
    /// </summary>
    public TranslationPage()
    {
        InitializeComponent();
        ViewModel = new TranslationPageViewModel();
        ViewModel.PropertyChanged += OnViewModelPropertyChangedAsync;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChangedAsync;
        ViewModel.Dispose();
    }

    /// <summary>
    /// 重置焦点.
    /// </summary>
    private void ResetFocus()
        => SourceBox?.Focus(FocusState.Programmatic);

    private async void OnViewModelPropertyChangedAsync(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.OutputText) && !string.IsNullOrEmpty(ViewModel.OutputText))
        {
            OutputBox.Focus(FocusState.Programmatic);
        }
        else if (e.PropertyName == nameof(ViewModel.IsInitialized) && ViewModel.IsInitialized)
        {
            await Task.Delay(200);
            ResetFocus();
        }
    }

    private void OnSourceBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down
                || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.TranslateCommand.Execute(default);
            }
        }
    }
}

/// <summary>
/// 翻译页面的基类.
/// </summary>
public abstract class TranslationPageBase : PageBase<TranslationPageViewModel>
{
}
