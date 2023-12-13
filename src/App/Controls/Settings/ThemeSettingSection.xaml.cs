// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Settings;

/// <summary>
/// 主题设置.
/// </summary>
public sealed partial class ThemeSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeSettingSection"/> class.
    /// </summary>
    public ThemeSettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var index = ViewModel.AppTheme switch
        {
            ElementTheme.Light => 0,
            ElementTheme.Dark => 1,
            _ => 2,
        };

        ThemePicker.SelectedIndex = index;
    }

    private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var theme = ThemePicker.SelectedIndex switch
        {
            0 => ElementTheme.Light,
            1 => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };

        ViewModel.AppTheme = theme;
        AppViewModel.Instance.ChangeTheme(theme);
    }
}
