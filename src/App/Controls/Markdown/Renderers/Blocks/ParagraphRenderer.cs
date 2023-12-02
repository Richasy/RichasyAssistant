// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Syntax;
using Microsoft.UI.Xaml.Documents;

namespace RichasyAssistant.App.Controls.Markdown.Renderers.Blocks;

internal sealed class ParagraphRenderer : WinUIObjectRenderer<ParagraphBlock>
{
    protected override void Write(WinUIRenderer renderer, ParagraphBlock obj)
    {
        var paragraph = new Paragraph();
        renderer.Add(paragraph);
        renderer.WriteLeafInline(obj);
        renderer.ExtractLastElementAsChild();
    }
}
