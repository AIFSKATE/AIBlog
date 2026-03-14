using Blazor.Dialog;
using Domain.Post;
using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminPostManager
    {
        [Parameter]
        public int? Id { get; set; }

        private string OriginalHtmlText = string.Empty;

        private bool IsProcessing = false;
        private string InputText
        {
            get => OriginalHtmlText;
            set
            {
                OriginalHtmlText = value;
                Console.WriteLine(OriginalHtmlText);
                HtmlText = Markdig.Markdown.ToHtml(value);
            }
        }
        private string HtmlText = string.Empty;

        List<TagDTO> AllTags = [];
        List<CategoryDTO> AllCategories = [];

        List<TagDTO> PostTags = [];
        int PostCategory = 0;

        IReadOnlyList<int> TagsSelected = new List<int>();

        PostDTO PostDTO = new();
        private string PostTitle { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (Id.HasValue)
            {
                PostDTO = await HttpClient.GetFromJsonAsync<PostDTO>($"Post/AdminGetPost?postId={Id.Value}");
                InputText = PostDTO.Markdown ?? string.Empty;
            }
            PostCategory = PostDTO.CategoryId ?? -1;
            PostTags = PostDTO.Tags ?? new List<TagDTO>();
            PostTitle = PostDTO.Title ?? string.Empty;
            IsProcessing = false;

            await ReloadTagsAsync();
            await ReloadCategoriesAsync();
        }



        private async Task ReloadTagsAsync()
        {
            AllTags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags") ?? [];

            TagsSelected = PostTags.Select(t => t.Id).ToList();
        }

        private void OnTagsChanged(IEnumerable<int> selectedIds)
        {
            // 【前 -> 后】同步：用户在界面点选后，直接更新 TagsSelected
            TagsSelected = selectedIds.ToList();

            // 同时更新 PostTags 列表，确保提交到后端的 DTO 是完整的
            PostTags = AllTags.Where(t => TagsSelected.Contains(t.Id)).ToList();
        }

        private async Task ReloadCategoriesAsync()
        {
            AllCategories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories") ?? new();
        }

        private void OnCategoryChanged(int selectedId)
        {
            PostCategory = selectedId;
        }


        private async Task OpenAddTagDialog()
        {
            var parameters = new DialogParameters<TagEditDialog>
            {
                { x => x.Title, "新增标签" },
                { x => x.Color, Color.Primary },
                { x => x.Model, new TagDTO() }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Small };
            var dialog = await DialogService.ShowAsync<TagEditDialog>("Add", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var tagData = (TagDTO)result.Data;
                if (string.IsNullOrWhiteSpace(tagData.TagName)) return;

                // 匹配后端接口: [HttpPost] AddTag([FromBody] string tagName)
                var response = await HttpClient.PostAsJsonAsync("Tag/AddTag", tagData.TagName);
                if (response.IsSuccessStatusCode)
                {
                    Snackbar.Add("标签新增成功", Severity.Success);
                    await ReloadTagsAsync(); // 刷新列表 
                }
            }
        }

        private async Task OpenAddCategoryDialog()
        {
            var parameters = new DialogParameters<CategoryEditDialog>
            {
                { x => x.Title, "新增分类" },
                { x => x.Color, Color.Secondary },
                { x => x.Model, new CategoryDTO() }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Small };
            var dialog = await DialogService.ShowAsync<CategoryEditDialog>("Add", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var categoryData = (CategoryDTO)result.Data;

                // 匹配后端接口: [HttpPost] AddCategory(CategoryDTO categoryDTO)
                var response = await HttpClient.PostAsJsonAsync("Category/AddCategory", categoryData);
                if (response.IsSuccessStatusCode)
                {
                    Snackbar.Add("分类新增成功", Severity.Success);
                    await ReloadCategoriesAsync(); // 刷新列表 
                }
            }
        }

        private async Task OnSubmit()
        {
            // --- 1. 数据合法性校验 ---
            if (string.IsNullOrWhiteSpace(PostTitle))
            {
                // 弹出错误提示，Severity.Error 会显示红色的警告框
                Snackbar.Add("文章标题不能为空，请输入标题后重试。", Severity.Error);
                return; // 关键：直接拦截，不执行后面的提交代码
            }

            if (string.IsNullOrWhiteSpace(InputText))
            {
                Snackbar.Add("文章为空，请重试。", Severity.Error);
                return;
            }
            IsProcessing = true;
            PostDTO.Title = PostTitle;
            PostDTO.CategoryId = PostCategory > -1 ? PostCategory : null;
            PostDTO.Tags = PostTags;
            PostDTO.Markdown = InputText;

            HttpResponseMessage response;
            // 4. 调用接口
            if (Id.HasValue)
            {
                // 编辑模式：更新文章
                response = await HttpClient.PutAsJsonAsync<PostDTO>("Post/UpdatePost", PostDTO);
            }
            else
            {
                // 新建模式：创建文章
                response = await HttpClient.PostAsJsonAsync("Post/CreatePost", new PostCreation()
                {
                    Title = PostTitle,
                    Markdown = InputText,
                    TagIDs = PostTags.Select(t => t.Id).ToList(), // 提取 ID 列表
                    CategoryId = PostCategory,
                });
            }
            // 无论成功与否，都要重新启用按钮
            // --- 处理结果反馈 ---
            if (response.IsSuccessStatusCode)
            {
                // 成功：显示绿色提示
                Snackbar.Add(Id.HasValue ? "文章更新成功！" : "文章发布成功！", Severity.Success);
                await Utils.NavigateTo("/admin/posts");
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();

                Snackbar.Add($"操作失败：状态码 {response.StatusCode}", Severity.Error);
            }
            IsProcessing = false;
        }
    }
}