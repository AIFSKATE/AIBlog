using Mapper.DTO;
using System.Net.Http.Json;

namespace Blazor.Pages.Tags
{
    public partial class Tags
    {
        private List<TagDTO> PostTags;

        protected override async Task OnInitializedAsync()
        {
            // 获取数据
            PostTags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
            await InvokeAsync(StateHasChanged);
        }
    }
}
