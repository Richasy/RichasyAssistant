// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Database;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
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
        var localSecretDbPath = Path.Combine(localPath, "Assets/Database/secret.db");
        GlobalSettings.Set(SettingNames.LibraryFolderPath, string.Empty);
        GlobalSettings.Set(SettingNames.DefaultChatDbPath, localChatDbPath);
        GlobalSettings.Set(SettingNames.DefaultSecretDbPath, localSecretDbPath);
    }

    /// <summary>
    /// 读取密钥数据库.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task RetrieveSecretsAsync()
    {
        var dbContext = await GetSecretDbContextAsync();
        if (dbContext == null)
        {
            return;
        }

        using (dbContext)
        {
            var metas = dbContext.Metadata.ToList();
            RetrieveSecret(metas, SettingNames.AzureOpenAIAccessKey);
            RetrieveSecret(metas, SettingNames.AzureOpenAIEndpoint);
            RetrieveSecret(metas, SettingNames.AzureOpenAIChatModelName);
            RetrieveSecret(metas, SettingNames.AzureOpenAICompletionModelName);
            RetrieveSecret(metas, SettingNames.AzureOpenAIEmbeddingModelName);

            RetrieveSecret(metas, SettingNames.OpenAIAccessKey);
            RetrieveSecret(metas, SettingNames.OpenAICustomEndpoint);
            RetrieveSecret(metas, SettingNames.OpenAIOrganization);
            RetrieveSecret(metas, SettingNames.OpenAIChatModelName);
            RetrieveSecret(metas, SettingNames.OpenAICompletionModelName);
            RetrieveSecret(metas, SettingNames.OpenAIEmbeddingModelName);
        }
    }

    /// <summary>
    /// 重置密钥数据库.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task ResetSecretsAsync()
    {
        var dbContext = await GetSecretDbContextAsync();
        if (dbContext == null)
        {
            return;
        }

        using(dbContext)
        {
            WriteSecret(dbContext, SettingNames.AzureOpenAIAccessKey);
            WriteSecret(dbContext, SettingNames.AzureOpenAIEndpoint);
            WriteSecret(dbContext, SettingNames.AzureOpenAIChatModelName);
            WriteSecret(dbContext, SettingNames.AzureOpenAICompletionModelName);
            WriteSecret(dbContext, SettingNames.AzureOpenAIEmbeddingModelName);

            WriteSecret(dbContext, SettingNames.OpenAIAccessKey);
            WriteSecret(dbContext, SettingNames.OpenAICustomEndpoint);
            WriteSecret(dbContext, SettingNames.OpenAIOrganization);
            WriteSecret(dbContext, SettingNames.OpenAIChatModelName);
            WriteSecret(dbContext, SettingNames.OpenAICompletionModelName);
            WriteSecret(dbContext, SettingNames.OpenAIEmbeddingModelName);
        }
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

    /// <summary>
    /// 获取密钥数据库上下文.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private static async Task<SecretDbContext> GetSecretDbContextAsync()
    {
        var libraryPath = GlobalSettings.TryGet<string>(SettingNames.LibraryFolderPath);
        if (string.IsNullOrEmpty(libraryPath))
        {
            return default;
        }

        var localDbPath = Path.Combine(libraryPath, "_secret_.db");
        if (!File.Exists(localDbPath))
        {
            var defaultDbPath = GlobalSettings.TryGet<string>(SettingNames.DefaultSecretDbPath);
            if (string.IsNullOrEmpty(defaultDbPath)
                || !File.Exists(defaultDbPath))
            {
                return default;
            }

            await Task.Run(() =>
            {
                File.Copy(defaultDbPath, localDbPath, true);
            });
        }

        var context = new SecretDbContext(localDbPath);
        return context;
    }

    private static void RetrieveSecret(List<Metadata> metas, SettingNames name)
    {
        var meta = metas.FirstOrDefault(p => p.Id == name.ToString());
        if (meta != null)
        {
            SettingsToolkit.WriteLocalSetting(name, meta.Value);
        }
    }

    private static void WriteSecret(SecretDbContext context, SettingNames name)
    {
        var value = SettingsToolkit.ReadLocalSetting(name, string.Empty);
        var source = context.Metadata.FirstOrDefault(p => p.Id == name.ToString());
        if (source != null)
        {
            source.Value = value;
            context.Metadata.Update(source);
        }
        else
        {
            source = new Metadata
            {
                Id = name.ToString(),
                Value = value,
            };
            context.Metadata.Add(source);
        }
    }

    private static void UpdateGlobalSetting<T>(SettingNames name, T defaultValue)
    {
        var setting = SettingsToolkit.ReadLocalSetting(name, defaultValue);
        GlobalSettings.Set(name, setting);
    }
}
