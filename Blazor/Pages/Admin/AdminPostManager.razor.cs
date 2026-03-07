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
                PostDTO = await HttpClient.GetFromJsonAsync<PostDTO>($"Post/GetPost?postId={Id.Value}");
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
            //禁用按钮，防止重复提交
            IsProcessing = true;
            // --- 2. 同步基本信息 ---
            // 既然通过了校验，直接赋值即可
            PostDTO.Title = PostTitle;
            // 如果 PostCategory 是 -1 或 0（代表未选择），则设为 null
            PostDTO.CategoryId = PostCategory > 0 ? PostCategory : null;

            // 3. 处理标签
            // 因为你在 OnTagsChanged 里已经实时同步了 PostTags，这里直接赋值即可
            PostDTO.Tags = PostTags;

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
            IsProcessing = false;
            // --- 处理结果反馈 ---
            if (response.IsSuccessStatusCode)
            {
                // 成功：显示绿色提示
                Snackbar.Add(Id.HasValue ? "文章更新成功！" : "文章发布成功！", Severity.Success);
                await Utils.NavigateTo("/posts");
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();

                Snackbar.Add($"操作失败：状态码 {response.StatusCode}", Severity.Error);
                Console.WriteLine($"后端报错详情: {errorMsg}");
            }
        }
    }
}