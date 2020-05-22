using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Html;

namespace PBug.Utils
{
    public static class MarkdownHelper
    {
        private static MarkdownPipeline pipeline = makePipeline();
        public static HtmlString ToHtml(string markdown)
        {
            return new HtmlString(Markdown.ToHtml(markdown, pipeline));
        }

        private static MarkdownPipeline makePipeline()
        {
            MarkdownPipelineBuilder build = new MarkdownPipelineBuilder()
                .DisableHtml()
                .UseTaskLists()
                .UseEmphasisExtras();

            return build.Build();
        }
    }
}