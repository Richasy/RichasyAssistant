// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Syntax;

namespace RichasyAssistant.App.Controls.Markdown.Renderers.Blocks;

internal sealed class HtmlBlockRenderer : WinUIObjectRenderer<HtmlBlock>
{
    protected override void Write(WinUIRenderer renderer, HtmlBlock obj) => renderer.WriteLeafRawLines(obj);
}
