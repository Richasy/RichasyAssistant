// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Context;
using Windows.ApplicationModel;
using Windows.Storage;

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
    public static Task BeforeExitAsync() => Task.CompletedTask;

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
        UpdateGlobalSetting(SettingNames.DefaultAzureOpenAIChatModelName, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureOpenAICompletionModelName, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureOpenAIEmbeddingModelName, string.Empty);

        // 配置 Open AI 设置.
        UpdateGlobalSetting(SettingNames.OpenAIAccessKey, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAIOrganization, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
        UpdateGlobalSetting(SettingNames.DefaultOpenAIChatModelName, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAICompletionModelName, string.Empty);
        UpdateGlobalSetting(SettingNames.OpenAIEmbeddingModelName, string.Empty);

        // 配置 Azure 翻译设置.
        UpdateGlobalSetting(SettingNames.AzureTranslateKey, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureTranslateRegion, string.Empty);

        // 配置百度翻译设置.
        UpdateGlobalSetting(SettingNames.BaiduTranslateAppId, string.Empty);
        UpdateGlobalSetting(SettingNames.BaiduTranslateAppKey, string.Empty);

        // 配置 Azure 语音设置.
        UpdateGlobalSetting(SettingNames.AzureSpeechKey, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureSpeechRegion, string.Empty);

        // 配置 Azure Whisper 设置.
        UpdateGlobalSetting(SettingNames.AzureWhisperKey, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureWhisperEndpoint, string.Empty);
        UpdateGlobalSetting(SettingNames.DefaultAzureWhisperModelName, string.Empty);

        // 配置 Open AI Whisper 设置.
        UpdateGlobalSetting(SettingNames.OpenAIWhisperKey, string.Empty);

        // 配置 Azure 图像设置.
        UpdateGlobalSetting(SettingNames.AzureImageKey, string.Empty);
        UpdateGlobalSetting(SettingNames.AzureImageEndpoint, string.Empty);

        // 配置 Open AI 图像设置.
        UpdateGlobalSetting(SettingNames.OpenAIImageKey, string.Empty);

        // 配置存储设置.
        var localPath = Package.Current.InstalledPath;
        var localFolderPath = ApplicationData.Current.LocalFolder.Path;
        var localChatDbPath = Path.Combine(localPath, "Assets/Database/chat.db");
        var localSecretDbPath = Path.Combine(localPath, "Assets/Database/secret.db");
        var localTranslationDbPath = Path.Combine(localPath, "Assets/Database/trans.db");
        var localDrawDbPath = Path.Combine(localPath, "Assets/Database/draw.db");
        var libraryPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
        GlobalSettings.Set(SettingNames.LocalFolderPath, localFolderPath);
        GlobalSettings.Set(SettingNames.LibraryFolderPath, libraryPath);
        GlobalSettings.Set(SettingNames.DefaultChatDbPath, localChatDbPath);
        GlobalSettings.Set(SettingNames.DefaultSecretDbPath, localSecretDbPath);
        GlobalSettings.Set(SettingNames.DefaultTranslationDbPath, localTranslationDbPath);
        GlobalSettings.Set(SettingNames.DefaultDrawDbPath, localDrawDbPath);
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
            RetrieveSecret(metas, SettingNames.DefaultAzureOpenAIChatModelName);
            RetrieveSecret(metas, SettingNames.AzureOpenAICompletionModelName);
            RetrieveSecret(metas, SettingNames.AzureOpenAIEmbeddingModelName);

            RetrieveSecret(metas, SettingNames.OpenAIAccessKey);
            RetrieveSecret(metas, SettingNames.OpenAICustomEndpoint);
            RetrieveSecret(metas, SettingNames.OpenAIOrganization);
            RetrieveSecret(metas, SettingNames.DefaultOpenAIChatModelName);
            RetrieveSecret(metas, SettingNames.OpenAICompletionModelName);
            RetrieveSecret(metas, SettingNames.OpenAIEmbeddingModelName);

            RetrieveSecret(metas, SettingNames.AzureTranslateKey);
            RetrieveSecret(metas, SettingNames.AzureTranslateRegion);

            RetrieveSecret(metas, SettingNames.BaiduTranslateAppId);
            RetrieveSecret(metas, SettingNames.BaiduTranslateAppKey);

            RetrieveSecret(metas, SettingNames.AzureSpeechKey);
            RetrieveSecret(metas, SettingNames.AzureSpeechRegion);

            RetrieveSecret(metas, SettingNames.AzureWhisperKey);
            RetrieveSecret(metas, SettingNames.AzureWhisperEndpoint);
            RetrieveSecret(metas, SettingNames.DefaultAzureWhisperModelName);

            RetrieveSecret(metas, SettingNames.OpenAIWhisperKey);

            RetrieveSecret(metas, SettingNames.AzureImageKey);
            RetrieveSecret(metas, SettingNames.AzureImageEndpoint);

            RetrieveSecret(metas, SettingNames.OpenAIImageKey);

            LoadDefaultService();
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

        using (dbContext)
        {
            WriteSecret(dbContext, SettingNames.AzureOpenAIAccessKey);
            WriteSecret(dbContext, SettingNames.AzureOpenAIEndpoint);
            WriteSecret(dbContext, SettingNames.DefaultAzureOpenAIChatModelName);
            WriteSecret(dbContext, SettingNames.AzureOpenAICompletionModelName);
            WriteSecret(dbContext, SettingNames.AzureOpenAIEmbeddingModelName);

            WriteSecret(dbContext, SettingNames.OpenAIAccessKey);
            WriteSecret(dbContext, SettingNames.OpenAICustomEndpoint);
            WriteSecret(dbContext, SettingNames.OpenAIOrganization);
            WriteSecret(dbContext, SettingNames.DefaultOpenAIChatModelName);
            WriteSecret(dbContext, SettingNames.OpenAICompletionModelName);
            WriteSecret(dbContext, SettingNames.OpenAIEmbeddingModelName);

            WriteSecret(dbContext, SettingNames.AzureTranslateKey);
            WriteSecret(dbContext, SettingNames.AzureTranslateRegion);

            WriteSecret(dbContext, SettingNames.BaiduTranslateAppId);
            WriteSecret(dbContext, SettingNames.BaiduTranslateAppKey);

            WriteSecret(dbContext, SettingNames.AzureSpeechKey);
            WriteSecret(dbContext, SettingNames.AzureSpeechRegion);

            WriteSecret(dbContext, SettingNames.AzureWhisperKey);
            WriteSecret(dbContext, SettingNames.AzureWhisperEndpoint);
            WriteSecret(dbContext, SettingNames.DefaultAzureWhisperModelName);

            WriteSecret(dbContext, SettingNames.OpenAIWhisperKey);

            WriteSecret(dbContext, SettingNames.AzureImageKey);
            WriteSecret(dbContext, SettingNames.AzureImageEndpoint);

            WriteSecret(dbContext, SettingNames.OpenAIImageKey);

            _ = await dbContext.SaveChangesAsync();
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
    /// 修改主题.
    /// </summary>
    /// <param name="theme">主题类型.</param>
    public void ChangeTheme(ElementTheme theme)
    {
        if (ActivatedWindow == null)
        {
            return;
        }

        (ActivatedWindow.Content as FrameworkElement).RequestedTheme = theme;
        if (theme == ElementTheme.Dark)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
        }
        else if (theme == ElementTheme.Light)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
        }
        else
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = default;
        }
    }

    /// <summary>
    /// 获取密钥数据库上下文.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private static async Task<SecretDbContext> GetSecretDbContextAsync()
    {
        var libraryPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
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
            _ = context.Metadata.Update(source);
        }
        else
        {
            source = new Metadata
            {
                Id = name.ToString(),
                Value = value,
            };
            _ = context.Metadata.Add(source);
        }
    }

    private static void UpdateGlobalSetting<T>(SettingNames name, T defaultValue)
    {
        var setting = SettingsToolkit.ReadLocalSetting(name, defaultValue);
        GlobalSettings.Set(name, setting);
    }

    private static void LoadDefaultService()
    {
        if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIAccessKey, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultKernel, KernelType.AzureOpenAI);
        }
        else if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.OpenAIAccessKey, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultKernel, KernelType.OpenAI);
        }

        if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.AzureTranslateKey, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultTranslate, TranslateType.Azure);
        }
        else if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.BaiduTranslateAppId, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultTranslate, TranslateType.Baidu);
        }

        if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.AzureSpeechKey, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultSpeech, SpeechType.Azure);
        }
        else if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.AzureWhisperKey, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultSpeech, SpeechType.AzureWhisper);
        }
        else if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.OpenAIWhisperKey, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultSpeech, SpeechType.OpenAIWhisper);
        }

        if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.AzureImageKey, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultImage, DrawType.AzureDallE);
        }
        else if (!string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.OpenAIImageKey, string.Empty)))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultImage, DrawType.OpenAIDallE);
        }
    }
}
