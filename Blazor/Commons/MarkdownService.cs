using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;

public class MarkdownService
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownService()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .UseEmojiAndSmiley()
            .UseAdvancedExtensions()
            .Build();
    }

    public string ToHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;
        return Markdown.ToHtml(markdown, _pipeline);
    }

    // 这是一个全能方法，一次解析，吐出所有你想要的东西
    public (string Html, List<BlogHeader> Headers) GetParsedBlog(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return (string.Empty, new());

        var document = Markdown.Parse(markdown, _pipeline);

        // 提取标题逻辑
        var headers = document.Descendants<HeadingBlock>()
            .Select(h => new BlogHeader
            {
                Level = h.Level,
                Text = h.Inline?.FirstChild?.ToString() ?? "Untitled",
                Id = h.GetAttributes().Id
            }).ToList();

        // 渲染 HTML 逻辑
        var writer = new StringWriter();
        var renderer = new Markdig.Renderers.HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(document);

        return (writer.ToString(), headers);
    }
}

public class BlogHeader
{
    public int Level { get; set; }
    public string Text { get; set; }
    public string Id { get; set; }
}