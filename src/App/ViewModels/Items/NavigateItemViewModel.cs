// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.UI;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 导航项视图模型.
/// </summary>
public sealed partial class NavigateItemViewModel : DataViewModelBase<NavigateItem>
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private FluentSymbol _defaultIcon;

    [ObservableProperty]
    private FluentSymbol _selectedIcon;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItemViewModel"/> class.
    /// </summary>
    public NavigateItemViewModel(NavigateItem data)
        : base(data)
    {
        switch (data.Type)
        {
            case FeatureType.Chat:
                DefaultIcon = FluentSymbol.Chat;
                SelectedIcon = FluentSymbol.ChatFilled;
                break;
            case FeatureType.Draw:
                DefaultIcon = FluentSymbol.ImageSparkle;
                SelectedIcon = FluentSymbol.ImageSparkleFilled;
                break;
            case FeatureType.Translate:
                DefaultIcon = FluentSymbol.Translate;
                SelectedIcon = FluentSymbol.TranslateFilled;
                break;
            case FeatureType.Voice:
                DefaultIcon = FluentSymbol.MicSparkle;
                SelectedIcon = FluentSymbol.MicSparkleFilled;
                break;
            case FeatureType.Storage:
                DefaultIcon = FluentSymbol.BoxSearch;
                SelectedIcon = FluentSymbol.BoxSearchFilled;
                break;
            case FeatureType.Settings:
                DefaultIcon = FluentSymbol.Settings;
                SelectedIcon = FluentSymbol.SettingsFilled;
                break;
            default:
                break;
        }
    }
}
