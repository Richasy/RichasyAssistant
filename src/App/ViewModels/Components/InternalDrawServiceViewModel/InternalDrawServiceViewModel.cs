// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部绘图服务视图模型.
/// </summary>
public sealed partial class InternalDrawServiceViewModel : ViewModelBase
{
    [RelayCommand]
    private void Initialize(bool isAzureImage)
    {
        if (isAzureImage)
        {
            AzureImageKey = SettingsToolkit.ReadLocalSetting(SettingNames.AzureImageKey, string.Empty);
            AzureImageEndpoint = SettingsToolkit.ReadLocalSetting(SettingNames.AzureImageEndpoint, string.Empty);
        }
        else
        {
            OpenAIImageKey = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAIImageKey, string.Empty);
            OpenAICustomEndpoint = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
        }
    }
}
