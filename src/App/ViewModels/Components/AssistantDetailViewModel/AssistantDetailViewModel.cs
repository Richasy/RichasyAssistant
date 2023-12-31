// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RichasyAssistant.App.Controls.Dialogs;
using RichasyAssistant.App.ViewModels.Views;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 助手详情视图模型.
/// </summary>
public sealed partial class AssistantDetailViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantDetailViewModel"/> class.
    /// </summary>
    public AssistantDetailViewModel(ChatPageViewModel parentViewModel)
    {
        _parentViewModel = parentViewModel;
        _azureOpenAIModels = new List<ServiceMetadata>();
        _openAIModels = new List<ServiceMetadata>();
        DisplayModels = new ObservableCollection<ServiceMetadata>();
        AllKernels = new ObservableCollection<ServiceMetadata>();

        AttachIsRunningToAsyncCommand(p => IsModelLoading = p, InitializeModelsCommand);
    }

    [RelayCommand]
    private void Initialize(Assistant data)
    {
        Source = data;
        IsImageCropper = false;
        IsCreateMode = string.IsNullOrEmpty(data.Id);
        Name = data.Name;
        Description = data.Description;
        Instruction = data.Instruction;
        UseDefaultKernel = data.UseDefaultKernel;
        var avatarPath = ResourceToolkit.GetAssistantAvatarPath(data.Id);
        if (File.Exists(avatarPath))
        {
            Avatar = new BitmapImage(new Uri(avatarPath));
        }

        CheckSaveButtonEnabled();

        if (!UseDefaultKernel && AllKernels.Count == 0)
        {
            InitializeKernels();
        }
    }

    [RelayCommand]
    private void Discard()
    {
        Source = default;
        Name = string.Empty;
        Description = string.Empty;
        Instruction = string.Empty;
        Avatar = null;
        IsCreateMode = false;
    }

    [RelayCommand]
    private async Task InitializeModelsAsync()
    {
        try
        {
            var kernelType = SelectedKernel.Id == OpenAIId
                ? KernelType.OpenAI : KernelType.AzureOpenAI;

            var models = kernelType == KernelType.AzureOpenAI ? _azureOpenAIModels : _openAIModels;
            if (models.Count == 0)
            {
                IsConfigInvalid = !ChatKernel.IsConfigValid(kernelType);

                if (!IsConfigInvalid)
                {
                    var (chatModels, textCompletions, embeddings) = await ChatKernel.GetSupportModelsAsync(kernelType);
                    foreach (var item in chatModels)
                    {
                        models.Add(new ServiceMetadata(item.Id, item.Value));
                    }
                }
            }

            TryClear(DisplayModels);
            foreach (var item in models)
            {
                DisplayModels.Add(item);
            }

            IsConfigInvalid = DisplayModels.Count == 0;

            if (!IsConfigInvalid)
            {
                SelectedModel = !string.IsNullOrEmpty(Source.Model)
                    ? DisplayModels.FirstOrDefault(p => p.Id == Source.Model)
                    : DisplayModels.First();
            }
        }
        catch (Exception)
        {
            IsConfigInvalid = true;
        }
    }

    [RelayCommand]
    private async Task SaveAsync(MemoryStream avatarStream = default)
    {
        var assistant = IsCreateMode ? new Assistant(Name, Description, Instruction) : Source;
        if (IsCreateMode)
        {
            assistant.UseDefaultKernel = UseDefaultKernel;
        }
        else
        {
            assistant.Name = Name;
            assistant.Description = Description;
            assistant.Instruction = Instruction;
            assistant.UseDefaultKernel = UseDefaultKernel;
            if (assistant.UseDefaultKernel)
            {
                assistant.Model = string.Empty;
                assistant.Remark = string.Empty;
            }
        }

        if (!UseDefaultKernel)
        {
            var kernelType = KernelType.Custom;
            if (SelectedKernel.Id == AzureOpenAIId)
            {
                kernelType = KernelType.AzureOpenAI;
            }
            else if (SelectedKernel.Id == OpenAIId)
            {
                kernelType = KernelType.OpenAI;
            }

            assistant.Kernel = kernelType;
            if (assistant.Kernel == KernelType.Custom)
            {
                assistant.Model = SelectedKernel.Id;
                assistant.Remark = SelectedKernel.Name;
            }
            else if (SelectedModel != null)
            {
                assistant.Model = SelectedModel.Id;
                assistant.Remark = SelectedModel.Name;
            }
        }

        if (avatarStream != null)
        {
            avatarStream.Seek(0, SeekOrigin.Begin);
            var avatarPath = ResourceToolkit.GetAssistantAvatarPath(assistant.Id);
            if (!Directory.Exists(Path.GetDirectoryName(avatarPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(avatarPath));
            }

            await File.WriteAllBytesAsync(avatarPath, avatarStream.ToArray());
        }

        await ChatDataService.AddOrUpdateAssistantAsync(assistant);
        _parentViewModel.RefreshAssistantsCommand.Execute(default);
        Source = null;
        IsCreateMode = false;
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        var dialog = new TipDialog(ResourceToolkit.GetLocalizedString(StringNames.DeleteAssistantWarning));
        dialog.XamlRoot = AppViewModel.Instance.ActivatedWindow.Content.XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ChatDataService.DeleteAssistantAsync(Source.Id);
            _parentViewModel.InitializeCommand.Execute(default);
            Source = null;
        }
    }

    private void InitializeKernels()
    {
        TryClear(AllKernels);
        AllKernels.Add(new ServiceMetadata(AzureOpenAIId, "Azure Open AI"));
        AllKernels.Add(new ServiceMetadata(OpenAIId, "Open AI"));

        var extraServices = ChatDataService.GetExtraKernels();
        if (extraServices.Count > 0)
        {
            foreach (var metadata in extraServices)
            {
                AllKernels.Add(metadata);
            }
        }

        var defaultKernel = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultKernel, KernelType.AzureOpenAI);
        if (defaultKernel == KernelType.AzureOpenAI)
        {
            SelectedKernel = AllKernels.First();
        }
        else if (defaultKernel == KernelType.OpenAI)
        {
            SelectedKernel = AllKernels[1];
        }
        else
        {
            var chatKernelId = SettingsToolkit.ReadLocalSetting(SettingNames.CustomKernelId, string.Empty);
            if (!string.IsNullOrEmpty(chatKernelId))
            {
                SelectedKernel = AllKernels.FirstOrDefault(p => p.Id == chatKernelId);
            }
        }

        SelectedKernel ??= AllKernels.First();
    }

    private void CheckTitle()
    {
        Title = IsImageCropper
            ? ResourceToolkit.GetLocalizedString(StringNames.AvatarCropper)
            : IsCreateMode
                ? ResourceToolkit.GetLocalizedString(StringNames.CreateAssistant)
                : ResourceToolkit.GetLocalizedString(StringNames.EditAssistant);
    }

    private void CheckSaveButtonEnabled()
    {
        IsSaveButtonEnabled =
            !string.IsNullOrEmpty(Name)
            && !string.IsNullOrEmpty(Instruction)
            && !IsConfigInvalid;
    }

    partial void OnIsCreateModeChanged(bool value)
        => CheckTitle();

    partial void OnIsImageCropperChanged(bool value)
        => CheckTitle();

    partial void OnNameChanged(string value)
        => CheckSaveButtonEnabled();

    partial void OnInstructionChanged(string value)
        => CheckSaveButtonEnabled();

    partial void OnIsConfigInvalidChanged(bool value)
        => CheckSaveButtonEnabled();

    partial void OnUseDefaultKernelChanged(bool value)
    {
        if (!value)
        {
            InitializeKernels();
        }
        else
        {
            IsConfigInvalid = false;
        }
    }

    partial void OnSelectedKernelChanged(ServiceMetadata value)
    {
        if (value == null)
        {
            return;
        }

        if (value.Id == AzureOpenAIId || value.Id == OpenAIId)
        {
            InitializeModelsCommand.Execute(default);
        }
        else
        {
            IsConfigInvalid = false;
            SelectedModel = default;
            TryClear(DisplayModels);
        }
    }
}
