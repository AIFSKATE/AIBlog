using Mapper.DTO;
using Markdig;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminPostManager
    {
        [Parameter]
        public int? Id { get; set; }

        private string OriginalHtmlText = string.Empty;
        private string InputText
        {
            get
            {
                return OriginalHtmlText;
            }
            set
            {
                OriginalHtmlText = value;
                HtmlText = Markdig.Markdown.ToHtml(value);
            }
        }
        private string HtmlText = string.Empty;

        List<TagDTO> AllTags = new List<TagDTO>();
        List<CategoryDTO> AllCategories = new();

        List<TagDTO> PostTags;
        int PostCategory = 0;

        List<(bool, string)> TempoTags = new();
        List<(bool, string)> TempoCategories = new();

        PostDTO PostDTO = new PostDTO();

        protected override async Task OnInitializedAsync()
        {
            if (Id.HasValue)
            {
                PostDTO = await HttpClient.GetFromJsonAsync<PostDTO>($"Post/GetPost?postId={Id.Value}");
                InputText = PostDTO.Markdown??string.Empty;
            }
            PostCategory = PostDTO.CategoryId ?? 0;
            PostTags = PostDTO.Tags ?? new List<TagDTO>();

            AllTags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
            AllCategories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");

            TempoTags = AllTags.Select(t => (PostTags.Any(x => x.Id == t.Id), t.TagName)).ToList();
            TempoCategories = AllCategories.Select(t => (PostCategory == t.Id, t.CategoryName)).ToList();


        }

        private void OnSubmit()
        {
            Console.WriteLine("成功提交");
            PostDTO.Title = PostDTO.Title?.Trim();
            PostDTO.Markdown = InputText;
            for (int i = 0; i < TempoCategories.Count; i++)
            {
                if (TempoCategories[i].Item1)
                {
                    PostDTO.CategoryId = AllCategories[i].Id;
                    break;
                }
                else
                {
                    PostDTO.CategoryId = null;
                }
            }
            PostDTO.Tags = AllTags.Where((t, i) => TempoTags[i].Item1).ToList();
            if (Id.HasValue)
            {
                HttpClient.PutAsJsonAsync<PostDTO>("Post/UpdatePost", PostDTO);
            }
        }

    }
}
