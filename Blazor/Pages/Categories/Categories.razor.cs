using Mapper.DTO;
using System.Net.Http.Json;

namespace Blazor.Pages.Categories
{
    public partial class Categories
    {
        private List<CategoryDTO> categories;

        protected override async Task OnInitializedAsync()
        {
            // 获取数据，去掉了多余的 StateHasChanged
            categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
        }
    }
}