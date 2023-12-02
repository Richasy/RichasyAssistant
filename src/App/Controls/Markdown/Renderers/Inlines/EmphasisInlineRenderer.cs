﻿// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Syntax.Inlines;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Documents;
using Windows.UI.Text;

namespace RichasyAssistant.App.Controls.Markdown.Renderers.Inlines;

internal sealed class EmphasisInlineRenderer : WinUIObjectRenderer<EmphasisInline>
{
    protected override void Write(WinUIRenderer renderer, EmphasisInline obj)
    {
        var span = new Span();
        switch (obj.DelimiterChar)
        {
            case '*':
            case '_':
                span.FontWeight = obj.DelimiterCount >= 2 ? FontWeights.Bold : FontWeights.Normal;
                span.FontStyle = obj.DelimiterCount is 1 or 3 ? FontStyle.Italic : FontStyle.Normal;
                break;
            case '~':
                span.TextDecorations = obj.DelimiterCount == 2 ? TextDecorations.Strikethrough : TextDecorations.None;
                break;
            default:
                break;
        }

        renderer.Add(span);
        renderer.WriteChildren(obj);
        renderer.ExtractSpanAsChild();
    }
}
