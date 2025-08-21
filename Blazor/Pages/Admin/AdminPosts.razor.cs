using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;

namespace Blazor.Pages.Admin
{
    public partial class AdminPosts
    {
        [Parameter]
        public int? page { get; set; }
        private int Limit = 15;
        private int TotalPage;
        private QueryPostsDto PostsData;

        protected override async Task OnInitializedAsync()
        {
            //var token = await Common.GetStorageAsync("token");
            //Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            page = page.HasValue ? page : 1;
            await RenderPage(page);
        }

        private async Task RenderPage(int? page)
        {
            this.page = page;
            // 获取数据
            PostsData = await HttpClient.GetFromJsonAsync<QueryPostsDto>($"Post/QueryPosts?page={page}&limit={Limit}");
            // 计算总页码
            TotalPage = (int)Math.Ceiling((PostsData.Count / (double)Limit));
            await InvokeAsync(StateHasChanged);
        }

        private async Task DeleteAsync(int id)
        {
            // 弹窗确认
            bool confirmed = await Utils.InvokeAsync<bool>("confirm", "\n💥💢真的要干掉这篇该死的文章吗💢💥");

            if (confirmed)
            {
                var response = await HttpClient.DeleteAsync($"Post/DeletePost?postId={id}");

                if (response.IsSuccessStatusCode)
                {
                    await RenderPage(page);
                }
            }
        }
    }
}
