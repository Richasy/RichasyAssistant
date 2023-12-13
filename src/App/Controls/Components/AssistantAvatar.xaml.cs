// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RichasyAssistant.Libs.Service;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 助理头像.
/// </summary>
public sealed partial class AssistantAvatar : UserControl
{
    /// <summary>
    /// <see cref="Id"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IdProperty =
        DependencyProperty.Register(nameof(Id), typeof(string), typeof(AssistantAvatar), new PropertyMetadata(default, new PropertyChangedCallback(OnIdChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantAvatar"/> class.
    /// </summary>
    public AssistantAvatar()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 助理标识符.
    /// </summary>
    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    /// <summary>
    /// 更新.
    /// </summary>
    public void Update()
    {
        if (string.IsNullOrEmpty(Id))
        {
            ShowPlaceholder();
            return;
        }

        var avatarPath = ResourceToolkit.GetAssistantAvatarPath(Id);
        if (File.Exists(avatarPath))
        {
            AvatarImage.Source = default;
            ShowAvatar();
            AvatarImage.Source = avatarPath;
        }
        else
        {
            ShowPlaceholder();
        }

        var assistant = ChatDataService.GetAssistant(Id);
        if (assistant != null)
        {
            ToolTipService.SetToolTip(this, assistant.Name);
        }
        else
        {
            ToolTipService.SetToolTip(this, default);
        }

        void ShowPlaceholder()
        {
            PlaceholderIcon.Visibility = Visibility.Visible;
            AvatarImage.Visibility = Visibility.Collapsed;
        }

        void ShowAvatar()
        {
            AvatarImage.Visibility = Visibility.Visible;
            PlaceholderIcon.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 设置头像源.
    /// </summary>
    /// <param name="image">图片.</param>
    public void SetSource(BitmapImage image)
    {
        AvatarImage.Source = image;
        AvatarImage.Visibility = Visibility.Visible;
        PlaceholderIcon.Visibility = Visibility.Collapsed;
    }

    private static void OnIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AssistantAvatar;
        instance.Update();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Update();
}
