// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using Windows.ApplicationModel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    private AppViewModel()
        => ResetGlobalSettings();

    /// <summary>
    /// 在应用退出前执行.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static Task BeforeExitAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 重置全局设置.
    /// </summary>
    public static void ResetGlobalSettings()
    {
        // 配置默认设置.
        UpdateGlobalSetting(SettingNames.DefaultTopP, 0d);
        UpdateGlobalSetting(SettingNames.DefaultTemperature, 0.5d);
        UpdateGlobalSetting(SettingNames.DefaultFrequencyPenalty, 0d);
        UpdateGlobalSetting(SettingNames.DefaultPresencePenalty, 0d);
        UpdateGlobalSetting(SettingNames.DefaultMaxResponseTokens, 1000);

        // 配置 Azure OpenAI 设置.
        UpdateGlobalSetting(SettingNames.AzureOpenAIAccessKey, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureOpenAIEndpoint, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureOpenAIChatModelName, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureOpenAICompletionModelName, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureOpenAIEmbeddingModelName, string.Empty);

        // 配置 Open AI 设置.
        UpdateGlobalSetting(SettingNames.OpenAIAccessKey, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAIOrganization, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAIChatModelName, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAICompletionModelName, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAIEmbeddingModelName, string.Empty);

        // 配置存储设置.
        var localPath = Package.Current.InstalledPath;
        var localChatDbPath = Path.Combine(localPath, "Assets/Database/chat.db");
        GlobalSettings.Set(SettingNames.LibraryFolderPath, string.Empty);
        GlobalSettings.Set(SettingNames.DefaultChatDbPath, localChatDbPath);
    }

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="message">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(string message, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotification(message, type, ActivatedWindow));

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="messageName">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(StringNames messageName, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotification(ResourceToolkit.GetLocalizedString(messageName), type, ActivatedWindow));

    private static void UpdateGlobalSetting<T>(SettingNames name, T defaultValue)
    {
        var setting = SettingsToolkit.ReadLocalSetting(name, defaultValue);
        GlobalSettings.Set(name, setting);
    }
}
