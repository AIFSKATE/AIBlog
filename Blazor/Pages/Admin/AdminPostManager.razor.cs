using Domain.Post;
using Mapper.DTO;
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
            get => OriginalHtmlText;
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

        // --- 弹窗控制状态 ---
        private bool ShowModal = false;
        private bool IsAddingTag = true;
        private string NewItemName = string.Empty;
        private string NewCategoryDesc = string.Empty;

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

            await ReloadTagsAsync();
            await ReloadCategoriesAsync();
        }

        // --- 弹窗相关方法 ---
        private void OpenTagModal()
        {
            IsAddingTag = true;
            NewItemName = string.Empty;
            ShowModal = true;
        }

        private void OpenCategoryModal()
        {
            IsAddingTag = false;
            NewItemName = string.Empty;
            NewCategoryDesc = string.Empty;
            ShowModal = true;
        }

        private void CloseModal()
        {
            ShowModal = false;
        }


        private void SetCategory(int index)
        {
            // 将所有选中状态重置为 0
            for (int i = 0; i < CtgriesSelected.Count; i++)
            {
                CtgriesSelected[i] = 0;
            }
            // 将当前点击的设为 1
            CtgriesSelected[index] = 1;
        }

        private async Task ConfirmAdd()
        {
            if (string.IsNullOrWhiteSpace(NewItemName)) return;

            if (IsAddingTag)
            {
                // Tag Controller 期望 [FromBody] string，直接传值
                var res = await HttpClient.PostAsJsonAsync("Tag/AddTag", NewItemName);
                if (res.IsSuccessStatusCode)
                {
                    await ReloadTagsAsync();
                }
            }
            else
            {
                // Category Controller 期望 CategoryDTO
                var newCat = new CategoryDTO { CategoryName = NewItemName, Description = NewCategoryDesc };
                var res = await HttpClient.PostAsJsonAsync("Category/AddCategory", newCat);
                if (res.IsSuccessStatusCode)
                {
                    await ReloadCategoriesAsync();
                }
            }

            CloseModal();
        }

        // --- 抽离的刷新数据方法（保证不丢状态） ---
        private async Task ReloadTagsAsync()
        {
            // 记住当前选中的 Tag ID
            var oldSelectedTagIds = AllTags.Where((t, i) => TagsSelected.Count > i && TagsSelected[i] == 1).Select(t => t.Id).ToList();
            if (PostTags.Any()) oldSelectedTagIds.AddRange(PostTags.Select(p => p.Id));

            AllTags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags") ?? new();
            TagNames = AllTags.Select(t => t.TagName).ToList();

            // 恢复选中状态
            TagsSelected = AllTags.Select(t => oldSelectedTagIds.Contains(t.Id) ? 1 : 0).ToList();
        }

        private async Task ReloadCategoriesAsync()
        {
            // 记住当前选中的 Category ID
            int oldSelectedCategoryId = PostCategory;
            var index = CtgriesSelected.IndexOf(1);
            if (index >= 0 && index < AllCategories.Count)
            {
                oldSelectedCategoryId = AllCategories[index].Id;
            }

            AllCategories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories") ?? new();
            CtgryNames = AllCategories.Select(c => c.CategoryName).ToList();

            // 恢复选中状态
            CtgriesSelected = AllCategories.Select(c => c.Id == oldSelectedCategoryId ? 1 : 0).ToList();
        }

        // --- 原有提交逻辑 ---
        private void OnSubmit()
        {
            // ... (保持你原有的 OnSubmit 不变)
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