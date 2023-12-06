// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Locator;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部内核视图模型.
/// </summary>
public sealed partial class InternalKernelViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalKernelViewModel"/> class.
    /// </summary>
    public InternalKernelViewModel()
    {
        AzureOpenAIChatModelCollection = new ObservableCollection<string>();
        OpenAIChatModelCollection = new ObservableCollection<string>();
        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand, TryLoadAIModelSourceCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync(bool isAzureOpenAI)
    {
        if (isAzureOpenAI)
        {
            AzureOpenAIAccessKey = SettingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIAccessKey, string.Empty);
            AzureOpenAIEndpoint = SettingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEndpoint, string.Empty);
        }
        else
        {
            OpenAIAccessKey = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAIAccessKey, string.Empty);
            OpenAICustomEndpoint = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
            OpenAIOrganization = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAIOrganization, string.Empty);
        }

        await TryLoadAIModelSourceAsync(isAzureOpenAI);
    }

    [RelayCommand]
    private async Task TryLoadAIModelSourceAsync(bool isAzureOpenAI)
    {
        if ((isAzureOpenAI && AzureOpenAIChatModelCollection.Count > 0)
            || (!isAzureOpenAI && OpenAIChatModelCollection.Count > 0)
            || (isAzureOpenAI && (string.IsNullOrEmpty(AzureOpenAIAccessKey) || string.IsNullOrEmpty(AzureOpenAIEndpoint)))
            || (!isAzureOpenAI && string.IsNullOrEmpty(OpenAIAccessKey)))
        {
            return;
        }

        try
        {
            if (isAzureOpenAI)
            {
                GlobalSettings.Set(SettingNames.AzureOpenAIAccessKey, AzureOpenAIAccessKey);
                GlobalSettings.Set(SettingNames.AzureOpenAIEndpoint, AzureOpenAIEndpoint);
                var (chatModels, textCompletions, embeddings) = await ChatKernel.GetSupportModelsAsync(KernelType.AzureOpenAI);
                TryClear(AzureOpenAIChatModelCollection);
                foreach (var item in chatModels)
                {
                    AzureOpenAIChatModelCollection.Add(item);
                }

                var localChatModelName = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultAzureOpenAIChatModelName, string.Empty);
                AzureOpenAIChatModelName = string.IsNullOrEmpty(localChatModelName)
                    ? chatModels.First()
                    : chatModels.FirstOrDefault(p => p.Equals(localChatModelName));
            }
            else
            {
                GlobalSettings.Set(SettingNames.OpenAIAccessKey, AzureOpenAIAccessKey);
                var (chatModels, textCompletions, embeddings) = await ChatKernel.GetSupportModelsAsync(KernelType.OpenAI);
                TryClear(OpenAIChatModelCollection);
                foreach (var item in chatModels)
                {
                    OpenAIChatModelCollection.Add(item);
                }

                var localChatModelName = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultOpenAIChatModelName, string.Empty);
                OpenAIChatModelName = string.IsNullOrEmpty(localChatModelName)
                    ? chatModels.First()
                    : chatModels.FirstOrDefault(p => p.Equals(localChatModelName));
            }
        }
        catch (Exception ex)
        {
            Logger.Debug(ex.Message);
        }
    }
}
