using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System.Text.RegularExpressions;

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

        var normalized = Regex.Replace(markdown, @"(?m)^(\s*```)C#$", "$1csharp", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"(?m)^(\s*```)C\+\+$", "$1cpp", RegexOptions.IgnoreCase);

        return Markdown.ToHtml(normalized, _pipeline);
    }

    public (string Html, List<BlogHeader> Headers) GetParsedBlog(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return (string.Empty, new());

        // 1. 必须先进行“暴力修复”，否则解析出的 Heading ID 没问题，但代码块高亮会失效
        var normalized = Regex.Replace(markdown, @"(?m)^(\s*```)C#$", "$1csharp", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"(?m)^(\s*```)C\+\+$", "$1cpp", RegexOptions.IgnoreCase);

        // 2. 解析文档
        var document = Markdown.Parse(normalized, _pipeline);

        // 3. 提取标题
        var headers = document.Descendants<HeadingBlock>()
            .Select(h => new BlogHeader
            {
                Level = h.Level,
                Text = h.Inline?.FirstChild?.ToString() ?? "Untitled",
                Id = h.GetAttributes().Id // 确保已经在 pipeline 里开启了 UseAutoIdentifiers
            }).ToList();

        // 4. 渲染 HTML (这里用 StringWriter 比较稳)
        using var writer = new StringWriter();
        var renderer = new Markdig.Renderers.HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(document);
        writer.Flush();

        return (writer.ToString(), headers);
    }
}

public class BlogHeader
{
    public int Level { get; set; }
    public string Text { get; set; }
    public string Id { get; set; }
}