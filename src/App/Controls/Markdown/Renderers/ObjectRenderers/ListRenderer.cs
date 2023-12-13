﻿// Copyright (c) Richasy Assistant. All rights reserved.
// <auto-generated />

using Markdig.Syntax;
using RichasyAssistant.App.Controls.Markdown.TextElements;

namespace RichasyAssistant.App.Controls.Markdown.Renderers.ObjectRenderers;

internal class ListRenderer : WinUIObjectRenderer<ListBlock>
{
    protected override void Write(WinUIRenderer renderer, ListBlock listBlock)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (listBlock == null)
        {
            throw new ArgumentNullException(nameof(listBlock));
        }

        var list = new MyList(listBlock);

        renderer.Push(list);

        foreach (var item in listBlock)
        {
            var listItemBlock = (ListItemBlock)item;
            var listItem = new MyBlockContainer(listItemBlock);
            renderer.Push(listItem);
            renderer.WriteChildren(listItemBlock);
            renderer.Pop();
        }

        renderer.Pop();
    }
}
