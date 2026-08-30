using Markdig;
using Gnezdow.MarkdigTextMate;
using Markdig.Parsers.Inlines;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TextMateSharp.Grammars;

namespace PBug.Utils
{
    public class MarkdownHelper
    {
        private readonly MarkdownPipeline _pipeline;

        public MarkdownHelper(IHttpContextAccessor ctxAccessor, LinkGenerator linkGenerator)
        {
            MarkdownPipelineBuilder build = new MarkdownPipelineBuilder()
                .DisableHtml()
                .UseAdvancedExtensions()
                .UseTextMate(new RegistryOptions(ThemeName.LightPlus));

            build.InlineParsers.InsertBefore<LinkInlineParser>(new PBugLinkInlineParser(ctxAccessor, linkGenerator));

            _pipeline = build.Build();
        }
        
        public HtmlString ToHtml(string markdown)
        {
            return new HtmlString(Markdown.ToHtml(markdown, _pipeline));
        }
    }
}