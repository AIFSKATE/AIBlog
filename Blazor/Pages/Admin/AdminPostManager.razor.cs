using Microsoft.AspNetCore.Components;

namespace Blazor.Pages.Admin
{
    public partial class AdminPostManager
    {
        [Parameter]
        public int? Id { get; set; }

        private string OriginalHtmlText = string.Empty;
        private string InputText {
            get
            {
                return OriginalHtmlText;
            }
            set
            {
                OriginalHtmlText=value;
                HtmlText=Markdig.Markdown.ToHtml(value);
            }
        }

        private string HtmlText = string.Empty;

        private void OnSubmit()
        {
            Console.WriteLine("成功提交");
        }

    }
}
