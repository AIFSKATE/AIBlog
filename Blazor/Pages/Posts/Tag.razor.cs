using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Blazor.Pages.Posts
{
    public partial class Tag
    {
        [Parameter] public int? page { get; set; }
        [Parameter] public int Id { get; set; }

        public string TagName { get; set; }
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
            page = newPage;
            await RenderPage(newPage);
        }

        private async Task RenderPage(int currentPage)
        {
            PostData = await HttpClient.GetFromJsonAsync<QueryPostsDto>($"Category/QueryPostsUnderCategory?Page={currentPage}&Limit={Limit}&categoryId={Id}");
            TagName = PostData.Info ?? string.Empty;
            TotalPage = (int)Math.Ceiling(PostData.Count / (double)Limit);
        }
    }
}
