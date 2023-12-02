﻿// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Syntax;
using Microsoft.UI.Xaml.Shapes;

namespace RichasyAssistant.App.Controls.Markdown.Renderers.Blocks;

internal sealed class HorizontalRuleRenderer : WinUIObjectRenderer<ThematicBreakBlock>
{
    protected override void Write(WinUIRenderer renderer, ThematicBreakBlock obj)
    {
        var context = renderer.Context;
        var rect = new Rectangle
        {
            Height = context.HorizontalRuleThickness,
            Margin = context.HorizontalRuleMargin,
            Fill = context.HorizontalRuleBrush,
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
        };

        renderer.Add(rect);
        renderer.ExtractLastElementAsChild();
    }
}
