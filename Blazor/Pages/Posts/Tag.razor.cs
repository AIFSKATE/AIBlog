using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Blazor.Pages.Posts
{
    public partial class Tag
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        [Parameter]
        public int? page { get; set; }
        [Parameter]
        public int Id { get; set; }

        public string TagName { get; set; }
        private int TotalPage;
        private int Limit = 15;

        private QueryPostDto PostData;


        protected override async Task OnInitializedAsync()
        {
            // 设置默认值
            page = page.HasValue ? page : 1;

            await RenderPage(page);
        }

        private async Task RenderPage(int? page)
        {
            this.page = page;
            PostData = await HttpClient.GetFromJsonAsync<QueryPostDto>($"Tag/QueryPostsUnderTag?tagId={Id}&Page={page}&Limit={Limit}");
            TagName= PostData.Info;
            TotalPage = (int)Math.Ceiling((PostData.Count / (double)Limit));
            await InvokeAsync(StateHasChanged);
        }


        /// <summary>
        /// 初始化完成后执行
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            if (!page.HasValue)
            {
                page = 1;
                await RenderPage(page);
            }
        }
    }
}
