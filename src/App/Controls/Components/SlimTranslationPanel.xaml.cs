// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using Microsoft.UI.Input;
using RichasyAssistant.App.ViewModels.Components;
using Windows.UI.Core;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 简化翻译面板.
/// </summary>
public sealed partial class SlimTranslationPanel : TranslationPanelBase
{
    /// <summary>
    /// <see cref="LeftElement"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty LeftElementProperty =
        DependencyProperty.Register(nameof(LeftElement), typeof(object), typeof(SlimChatSessionPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="RightElement"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty RightElementProperty =
        DependencyProperty.Register(nameof(RightElement), typeof(object), typeof(SlimChatSessionPanel), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="SlimTranslationPanel"/> class.
    /// </summary>
    public SlimTranslationPanel()
    {
        InitializeComponent();
        RegisterPropertyChangedCallback(VisibilityProperty, OnVisibilityChanged);
    }

    /// <summary>
    /// 头部左侧插件.
    /// </summary>
    public object LeftElement
    {
        get => (object)GetValue(LeftElementProperty);
        set => SetValue(LeftElementProperty, value);
    }

    /// <summary>
    /// 头部右侧插件.
    /// </summary>
    public object RightElement
    {
        get => (object)GetValue(RightElementProperty);
        set => SetValue(RightElementProperty, value);
    }

    /// <summary>
    /// 重置焦点.
    /// </summary>
    public void ResetFocus()
        => SourceBox?.Focus(FocusState.Programmatic);

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is TranslationViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChangedAsync;
        }

        if (e.NewValue == null)
        {
            return;
        }

        var vm = e.NewValue as TranslationViewModel;
        vm.PropertyChanged += OnViewModelPropertyChangedAsync;
        if (IsLoaded)
        {
            ResetFocus();
        }
    }

    private void OnVisibilityChanged(DependencyObject sender, DependencyProperty dp)
        => ResetFocus();

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
/// 翻译面板基类.
/// </summary>
public abstract class TranslationPanelBase : ReactiveUserControl<TranslationViewModel>
{
}
