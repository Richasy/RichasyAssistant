﻿// Copyright (c) Richasy Assistant. All rights reserved.
// <auto-generated />

using Markdig.Extensions.TaskLists;
using RichasyAssistant.App.Controls.Markdown.TextElements;

namespace RichasyAssistant.App.Controls.Markdown.Renderers.ObjectRenderers.Extensions;

internal class TaskListRenderer : WinUIObjectRenderer<TaskList>
{
    protected override void Write(WinUIRenderer renderer, TaskList taskList)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (taskList == null)
        {
            throw new ArgumentNullException(nameof(taskList));
        }

        var checkBox = new MyTaskListCheckBox(taskList);
        renderer.WriteInline(checkBox);
    }
}
