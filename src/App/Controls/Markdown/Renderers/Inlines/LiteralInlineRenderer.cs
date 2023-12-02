// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Syntax.Inlines;

namespace RichasyAssistant.App.Controls.Markdown.Renderers.Inlines;

internal sealed class LiteralInlineRenderer : WinUIObjectRenderer<LiteralInline>
{
    protected override void Write(WinUIRenderer renderer, LiteralInline obj)
    {
        if (obj.Content.IsEmpty)
        {
            return;
        }

        renderer.WriteText(ref obj.Content);
    }
}
