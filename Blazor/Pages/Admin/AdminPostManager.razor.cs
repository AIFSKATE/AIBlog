using AutoMapper;
using Domain.Post;
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

        List<TagDTO> AllTags = new();
        List<CategoryDTO> AllCategories = new();

        List<TagDTO> PostTags = new();
        int PostCategory = 0;

        List<string> TagNames = new();
        List<int> TagsSelected = new();
        List<string> CtgryNames = new();
        List<int> CtgriesSelected = new();

        PostDTO PostDTO = new();
        private string PostTitle { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (Id.HasValue)
            {
                PostDTO = await HttpClient.GetFromJsonAsync<PostDTO>($"Post/GetPost?postId={Id.Value}");
                InputText = PostDTO.Markdown ?? string.Empty;
            }
            PostCategory = PostDTO.CategoryId ?? -1;
            PostTags = PostDTO.Tags ?? new List<TagDTO>();
            PostTitle = PostDTO.Title ?? string.Empty;

            AllTags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
            AllCategories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");

            if (AllTags != null)
            {
                TagNames = AllTags.Select(t => t.TagName).ToList();
                TagsSelected = AllTags.Select(t => PostTags.Any(pt => pt.Id == t.Id) ? 1 : 0).ToList();
            }

            if (AllCategories != null)
            {
                CtgryNames = AllCategories.Select(c => c.CategoryName).ToList();
                CtgriesSelected = AllCategories.Select(c => c.Id == PostCategory ? 1 : 0).ToList();
            }
        }

        private void OnSubmit()
        {
            Console.WriteLine("成功提交");
            PostDTO.Title = PostTitle == string.Empty ? DateTime.Now.ToShortDateString() : PostTitle;
            PostDTO.Markdown = InputText;
            int index = CtgriesSelected.IndexOf(1);
            if (index < 0)
            {
                PostDTO.CategoryId = null;
            }
            else
            {
                PostDTO.CategoryId = AllCategories[index].Id;
            }
            PostDTO.Tags = AllTags.Where((t, i) => TagsSelected[i] == 1).ToList();
            if (Id.HasValue)
            {
                HttpClient.PutAsJsonAsync<PostDTO>("Post/UpdatePost", PostDTO);
            }
            else
            {
                HttpClient.PostAsJsonAsync("Post/CreatePost", new PostCreation()
                {
                    Title = PostDTO.Title ?? string.Empty,
                    Markdown = PostDTO.Markdown ?? string.Empty,
                    TagIDs = PostDTO.Tags?.Select(t => t.Id).ToList() ?? new List<int>(),
                    CategoryId = PostDTO.CategoryId,
                });
            }
        }

    }
}
