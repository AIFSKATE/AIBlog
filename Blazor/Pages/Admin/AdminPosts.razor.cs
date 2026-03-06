using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminPosts
    {
        [Parameter] public int? page { get; set; }
        private int Limit = 15;
        private int TotalPage;
        private QueryPostsDto PostsData;

        protected override async Task OnInitializedAsync()
        {
            page = page.HasValue ? page : 1;
            await RenderPage(page.Value);
        }

        private async Task OnPageChanged(int newPage)
        {
            page = newPage;
            await RenderPage(newPage);
        }

        private async Task RenderPage(int currentPage)
        {
            PostsData = await HttpClient.GetFromJsonAsync<QueryPostsDto>($"Post/QueryPosts?page={currentPage}&limit={Limit}");
            TotalPage = (int)Math.Ceiling(PostsData.Count / (double)Limit);
        }

        private async Task DeleteAsync(int id)
        {
            bool confirmed = await Utils.InvokeAsync<bool>("confirm", "\n💥💢真的要干掉这篇该死的文章吗💢💥");
            if (confirmed)
            {
                var response = await HttpClient.DeleteAsync($"Post/DeletePost?postId={id}");
                if (response.IsSuccessStatusCode)
                {
                    await RenderPage(page ?? 1);
                }
            }
        }
    }
}