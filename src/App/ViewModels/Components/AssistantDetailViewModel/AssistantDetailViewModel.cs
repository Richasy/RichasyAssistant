// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
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
    public AssistantDetailViewModel()
    {
        _azureOpenAIModels = new List<ServiceMetadata>();
        _openAIModels = new List<ServiceMetadata>();
        DisplayModels = new ObservableCollection<ServiceMetadata>();
        AllKernels = new ObservableCollection<ServiceMetadata>();
    }

    [RelayCommand]
    private void Initialize(Assistant data)
    {
        Source = data;
        Name = data.Name;
        Description = data.Description;
        Instruction = data.Instruction;
        UseDefaultKernel = data.UseDefaultKernel;
        var avatarPath = ResourceToolkit.GetAssistantAvatarPath(data.Id);
        if (File.Exists(avatarPath))
        {
            Avatar = new BitmapImage(new Uri(avatarPath));
        }
    }
}
