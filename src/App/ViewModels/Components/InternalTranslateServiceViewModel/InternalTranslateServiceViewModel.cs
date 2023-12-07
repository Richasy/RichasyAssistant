// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部翻译服务视图模型.
/// </summary>
public sealed partial class InternalTranslateServiceViewModel : ViewModelBase
{
    [RelayCommand]
    private void Initialize(bool isAzure)
    {
        if (isAzure)
        {
            AzureTranslateKey = SettingsToolkit.ReadLocalSetting(SettingNames.AzureTranslateKey, string.Empty);
            AzureTranslateRegion = SettingsToolkit.ReadLocalSetting(SettingNames.AzureTranslateRegion, string.Empty);
        }
        else
        {
            BaiduTranslateAppId = SettingsToolkit.ReadLocalSetting(SettingNames.BaiduTranslateAppId, string.Empty);
            BaiduTranslateKey = SettingsToolkit.ReadLocalSetting(SettingNames.BaiduTranslateAppKey, string.Empty);
        }
    }
}
