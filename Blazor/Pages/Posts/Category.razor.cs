using Domain.Account;
using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace Blazor.Pages.Posts
{
    public partial class Category
    {
        [Parameter] public int? page { get; set; }
        [Parameter] public int Id { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthStateTask { get; set; }

        public string categoryName { get; set; }
        private int TotalPage;
        private int Limit = 15;
        private QueryPostsDto PostData;

        protected override async Task OnInitializedAsync()
        {
            page = page.HasValue ? page : 1;
            await RenderPage(page.Value);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!page.HasValue)
            {
                page = 1;
                await RenderPage(page.Value);
            }
        }

        private async Task OnPageChanged(int newPage)
        {
            // Optional: You can navigate to update the URL like in AdminPosts
            // NavigationManager.NavigateTo($"/admin/category/{Id}/{newPage}");
            page = newPage;
            await RenderPage(newPage);
        }

        private async Task RenderPage(int currentPage)
        {
            var authState = await AuthStateTask;
            var user = authState.User;
            bool isAdmin = user.IsInRole(AIBlogRole.Admin);

            bool isAdminRoute = NavigationManager.Uri.Contains("/admin/category/");

            string action = (isAdmin && isAdminRoute)
                ? "AdminQueryPostsUnderCategory"
                : "QueryPostsUnderCategory";

            string requestUrl = $"Category/{action}?Page={currentPage}&Limit={Limit}&categoryId={Id}";

            PostData = await HttpClient.GetFromJsonAsync<QueryPostsDto>(requestUrl);

            if (PostData != null)
            {
                categoryName = PostData.Info ?? string.Empty;
                TotalPage = (int)Math.Ceiling(PostData.Count / (double)Limit);
            }
        }
    }
}