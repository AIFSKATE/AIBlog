using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Blazor.Pages.Posts
{
    public partial class Posts
    {
        [Parameter] public int? page { get; set; }

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

        // MudPagination 触发此事件，Blazor 会自动重绘UI，无需 StateHasChanged
        private async Task OnPageChanged(int newPage)
        {
            page = newPage;
            await RenderPage(newPage);
        }

        private async Task RenderPage(int currentPage)
        {
            PostData = await HttpClient.GetFromJsonAsync<QueryPostsDto>($"Post/QueryPosts?page={currentPage}&limit={Limit}");
            TotalPage = (int)Math.Ceiling(PostData.Count / (double)Limit);
        }
    }
}