﻿// Copyright (c) Richasy Assistant. All rights reserved.
// <auto-generated />

using Markdig.Syntax;
using RichasyAssistant.App.Controls.Markdown.TextElements;

namespace RichasyAssistant.App.Controls.Markdown.Renderers.ObjectRenderers;

internal class ThematicBreakRenderer : WinUIObjectRenderer<ThematicBreakBlock>
{
    protected override void Write(WinUIRenderer renderer, ThematicBreakBlock obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var thematicBreak = new MyThematicBreak(obj);

        renderer.WriteBlock(thematicBreak);
    }
}
