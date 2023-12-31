﻿// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Pages;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.UI;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 主视图模型.
/// </summary>
public sealed partial class MainViewModel
{
    private MainViewModel()
    {
        NavigateItems = new ObservableCollection<NavigateItemViewModel>
        {
            GetNavigateItem(FeatureType.Chat),
            GetNavigateItem(FeatureType.Draw),
            GetNavigateItem(FeatureType.Translate),
            GetNavigateItem(FeatureType.Voice),
            GetNavigateItem(FeatureType.Storage),
        };

        SettingsItem = GetNavigateItem(FeatureType.Settings);
    }

    /// <summary>
    /// 导航到指定页面.
    /// </summary>
    /// <param name="type">页面类型.</param>
    /// <param name="data">数据项.</param>
    public void NavigateTo(FeatureType type, object data = default)
    {
        SettingsItem.IsSelected = type == FeatureType.Settings;
        foreach (var item in NavigateItems)
        {
            item.IsSelected = item.Data.Type == type;
        }

        CurrentFeature = type;
        var pageType = type switch
        {
            FeatureType.Chat => typeof(ChatPage),
            FeatureType.Draw => typeof(DrawPage),
            FeatureType.Translate => typeof(TranslationPage),
            FeatureType.Voice => typeof(VoicePage),
            FeatureType.Storage => typeof(StoragePage),
            FeatureType.Settings => typeof(SettingsPage),
            _ => typeof(Page),
        };

        var args = new AppNavigateEventArgs(pageType, data);
        RequestNavigate?.Invoke(this, args);

        if (type != FeatureType.Settings)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastOpenedPage, type);
        }
    }

    private static NavigateItemViewModel GetNavigateItem(FeatureType type)
    {
        var item = new NavigateItem
        {
            Type = type,
            Name = type switch
            {
                FeatureType.Chat => ResourceToolkit.GetLocalizedString(StringNames.Chat),
                FeatureType.Draw => ResourceToolkit.GetLocalizedString(StringNames.Draw),
                FeatureType.Translate => ResourceToolkit.GetLocalizedString(StringNames.Translate),
                FeatureType.Voice => ResourceToolkit.GetLocalizedString(StringNames.Speech),
                FeatureType.Storage => ResourceToolkit.GetLocalizedString(StringNames.Storage),
                FeatureType.Settings => ResourceToolkit.GetLocalizedString(StringNames.Settings),
                _ => string.Empty,
            },
        };

        return new NavigateItemViewModel(item);
    }
}
