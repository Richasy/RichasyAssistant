// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;
using Windows.ApplicationModel.DataTransfer;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 聊天消息条目视图模型.
/// </summary>
public sealed partial class ChatMessageItemViewModel : DataViewModelBase<ChatMessage>
{
    private readonly Action<ChatMessage> _regenerateAction;
    private readonly Action<ChatMessage> _editAction;
    private readonly Action<ChatMessage> _deleteAction;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isUser;

    [ObservableProperty]
    private bool _isAssistant;

    [ObservableProperty]
    private string _time;

    [ObservableProperty]
    private bool _isLastMessage;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isRegenerateButtonShown;

    [ObservableProperty]
    private bool _useMarkdownRenderer;

    [ObservableProperty]
    private bool _isDefaultChat;

    [ObservableProperty]
    private string _assistantId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemViewModel"/> class.
    /// </summary>
    public ChatMessageItemViewModel(
        ChatMessage message,
        Action<ChatMessage> regenerateAction = default,
        Action<ChatMessage> editAction = default,
        Action<ChatMessage> deleteAction = default)
        : base(message)
    {
        Content = message.Content;
        IsAssistant = message.Role == ChatMessageRole.Assistant;
        IsUser = message.Role == ChatMessageRole.User;
        Time = message.Time.ToString("MM/dd HH:mm:ss");
        IsDefaultChat = string.IsNullOrEmpty(message.AssistantId);

        if (!IsDefaultChat)
        {
            AssistantId = message.AssistantId;
        }

        _regenerateAction = regenerateAction;
        _editAction = editAction;
        _deleteAction = deleteAction;
        UseMarkdownRenderer = SettingsToolkit.ReadLocalSetting(SettingNames.UseMarkdownRenderer, true);
        CheckRegenerateButtonState();
    }

    [RelayCommand]
    private void Copy()
    {
        var dp = new DataPackage();
        dp.SetText(Content);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(StringNames.Copied, InfoType.Success);
    }

    [RelayCommand]
    private void Regenerate()
        => _regenerateAction?.Invoke(Data);

    [RelayCommand]
    private void Edit()
    {
        if (string.IsNullOrEmpty(Content))
        {
            Delete();
            return;
        }

        Data.Content = Content;
        _editAction?.Invoke(Data);
    }

    [RelayCommand]
    private void Delete()
        => _deleteAction?.Invoke(Data);

    private void CheckRegenerateButtonState()
        => IsRegenerateButtonShown = !IsUser && IsLastMessage;

    partial void OnIsLastMessageChanged(bool value)
        => CheckRegenerateButtonState();
}
