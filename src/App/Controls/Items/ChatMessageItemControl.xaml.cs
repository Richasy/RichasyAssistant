// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 聊天消息条目.
/// </summary>
public sealed partial class ChatMessageItemControl : ChatMessageItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemControl"/> class.
    /// </summary>
    public ChatMessageItemControl()
    {
        InitializeComponent();
        MarkdownMessageBlock.Config = new Markdown.MarkdownConfig();
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ChatMessageItemViewModel vm)
        {
            _ = vm.IsUser
                ? VisualStateManager.GoToState(this, nameof(MyState), false)
                : VisualStateManager.GoToState(this, nameof(AssistantState), false);
        }
    }

    private void OnCardPointerEntered(object sender, PointerRoutedEventArgs e)
        => TryShowToolbar();

    private void OnCardPointerExited(object sender, PointerRoutedEventArgs e)
        => HideToolbar();

    private void OnCardPointerMoved(object sender, PointerRoutedEventArgs e)
        => TryShowToolbar();

    private void TryShowToolbar()
    {
        if (ViewModel.IsEditing)
        {
            return;
        }

        Toolbar.Visibility = Visibility.Visible;
    }

    private void HideToolbar()
        => Toolbar.Visibility = Visibility.Collapsed;

    private void OnEditorConfirmButtonClick(object sender, RoutedEventArgs e)
    {
        var text = Editor.Text;
        ViewModel.Content = text;
        ExitEditor();
        ViewModel.EditCommand.Execute(default);
    }

    private void OnEditorCancelButtonClick(object sender, RoutedEventArgs e)
        => ExitEditor();

    private void OnEditButtonClick(object sender, RoutedEventArgs e)
        => EnterEditor();

    private void EnterEditor()
    {
        HideToolbar();
        Editor.Text = ViewModel.Content;
        ViewModel.IsEditing = true;
        Editor.Focus(FocusState.Programmatic);
    }

    private void ExitEditor()
    {
        ViewModel.IsEditing = false;
        Editor.Text = string.Empty;
    }
}

/// <summary>
/// 聊天消息基类.
/// </summary>
public abstract class ChatMessageItemControlBase : ReactiveUserControl<ChatMessageItemViewModel>
{
}
