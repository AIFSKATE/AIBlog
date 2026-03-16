using Domain.Account;
using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace Blazor.Pages.Posts
{
    public partial class Post
    {
        [Parameter] public int Id { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthStateTask { get; set; }

        PostDTO? PostDto { get; set; }
        public bool Error = false;
        private bool isAdmin = false;
        private List<BlogHeader> headers = new();
        private string? renderedHtml;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // 1. 获取当前用户信息
                var authState = await AuthStateTask;
                var user = authState.User;
                isAdmin = user.IsInRole(AIBlogRole.Admin);

                // 2. 识别当前是否处于管理路由
                bool isAdminRoute = NavigationManager.Uri.Contains("/admin/post/");

                // 只有当 [是管理员] 且 [在管理路径下] 时，才调用 AdminGetPost
                // 这样即使你是管理员，只要走 /post/{Id} 路径，看到的也是游客过滤后的结果
                string requestUrl = (isAdmin && isAdminRoute)
                    ? $"Post/AdminGetPost?postId={Id}"
                    : $"Post/GetPost?postId={Id}";

                var response = await HttpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    PostDto = await response.Content.ReadFromJsonAsync<PostDTO>();
                    if (PostDto != null && !string.IsNullOrWhiteSpace(PostDto.Markdown))
                    {
                        var result = MarkdownService.GetParsedBlog(PostDto.Markdown);
                        renderedHtml = result.Html;
                        headers = result.Headers;
                    }
                }
                else
                {
                    // 如果游客访问已删除文章，后端返回 400/404，进入错误状态
                    Error = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载文章异常: {ex.Message}");
                Error = true;
            }
        }
    }
}