using Domain.Account;
using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization; // 必须引入
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

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // 2. 获取当前用户信息
                var authState = await AuthStateTask;
                var user = authState.User;
                isAdmin = user.IsInRole(AIBlogRole.Admin);
                // 3. 动态决定 API 路径
                // 如果是管理员，调用 AdminGetPost 确保能看到 IsDeleted == 1 的文章
                string requestUrl = isAdmin
                    ? $"/Post/AdminGetPost?postId={Id}"
                    : $"/Post/GetPost?postId={Id}";

                var response = await HttpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    PostDto = await response.Content.ReadFromJsonAsync<PostDTO>();
                }
                else
                {
                    // 如果游客尝试访问已删除文章，后端会返回 400/404，进入错误状态
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