// Copyright (c) Richasy Assistant. All rights reserved.

using System.Windows.Input;
using Microsoft.UI.Xaml.Media;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 功能项.
/// </summary>
public sealed partial class FeatureItem : UserControl
{
    /// <summary>
    /// <see cref="Icon"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(FeatureItem), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Text"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(FeatureItem), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Command"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(FeatureItem), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureItem"/> class.
    /// </summary>
    public FeatureItem() => InitializeComponent();

    /// <summary>
    /// 图标.
    /// </summary>
    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// 文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 命令.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}
