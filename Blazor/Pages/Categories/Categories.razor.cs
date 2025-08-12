using Mapper.DTO;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Blazor.Pages.Categories
{
    public partial class Categories
    {

        private List<CategoryDTO> categories;

        protected override async Task OnInitializedAsync()
        {
            // 获取数据
            categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Categoty/QueryCategories");
            await InvokeAsync(StateHasChanged);
        }
    }
}
