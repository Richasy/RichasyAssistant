// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Kernel;

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
        AzureOpenAIChatModelCollection = new ObservableCollection<Metadata>();
        OpenAIChatModelCollection = new ObservableCollection<Metadata>();
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

                var localChatModel = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultAzureOpenAIChatModel, "{}");
                var meta = JsonSerializer.Deserialize<Metadata>(localChatModel);
                AzureOpenAIChatModel = meta == null
                    ? chatModels.FirstOrDefault()
                    : chatModels.FirstOrDefault(p => p.Equals(meta));
            }
            else
            {
                GlobalSettings.Set(SettingNames.OpenAIAccessKey, OpenAIAccessKey);
                GlobalSettings.Set(SettingNames.OpenAIOrganization, OpenAICustomEndpoint);
                GlobalSettings.Set(SettingNames.OpenAIOrganization, OpenAIOrganization);
                var (chatModels, textCompletions, embeddings) = await ChatKernel.GetSupportModelsAsync(KernelType.OpenAI);
                TryClear(OpenAIChatModelCollection);
                foreach (var item in chatModels)
                {
                    OpenAIChatModelCollection.Add(item);
                }

                var localChatModel = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultOpenAIChatModelName, string.Empty);
                OpenAIChatModel = string.IsNullOrEmpty(localChatModel)
                    ? chatModels.FirstOrDefault()
                    : chatModels.FirstOrDefault(p => p.Id.Equals(localChatModel));
            }
        }
        catch (Exception ex)
        {
            LogException(ex);
        }
    }
}
