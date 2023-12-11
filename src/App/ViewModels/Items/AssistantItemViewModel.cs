// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 助手视图模型.
/// </summary>
public sealed partial class AssistantItemViewModel : DataViewModelBase<Assistant>
{
    [ObservableProperty]
    private string _avatar;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantItemViewModel"/> class.
    /// </summary>
    public AssistantItemViewModel(Assistant data)
        : base(data)
    {
        var avatarPath = ResourceToolkit.GetAssistantAvatarPath(data.Id);
        if (File.Exists(avatarPath))
        {
            Avatar = avatarPath;
        }
    }
}
