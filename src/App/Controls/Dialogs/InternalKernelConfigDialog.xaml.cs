// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Dialogs;

/// <summary>
/// 内核配置对话框.
/// </summary>
public sealed partial class InternalKernelConfigDialog : ContentDialog
{
    /// <summary>
    /// <see cref="IsAzureOpenAI"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsAzureOpenAIProperty =
        DependencyProperty.Register(nameof(IsAzureOpenAI), typeof(bool), typeof(InternalKernelConfigDialog), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalKernelConfigDialog"/> class.
    /// </summary>
    public InternalKernelConfigDialog(InternalKernelViewModel viewModel, bool isAzureOpenAI)
    {
        InitializeComponent();
        ViewModel = viewModel;
        IsAzureOpenAI = isAzureOpenAI;
        AppToolkit.ResetControlTheme(this);
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 获取或设置是否为 Azure Open AI.
    /// </summary>
    public bool IsAzureOpenAI
    {
        get => (bool)GetValue(IsAzureOpenAIProperty);
        set => SetValue(IsAzureOpenAIProperty, value);
    }

    private InternalKernelViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(IsAzureOpenAI);
}
