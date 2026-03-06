using Mapper.DTO;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminCategories
    {
        List<CategoryDTO> categories;
        bool Open { get; set; } = false;
        CategoryDTO UploadData = new();
        public Action<MouseEventArgs> OnConfirm { get; set; }
        public Action<MouseEventArgs> OnCancel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            OnCancel = Close;
            categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
        }

        protected async Task DeleteAsync(int id)
        {
            bool confirmed = await Utils.InvokeAsync<bool>("confirm", "\n💥💢真的要干掉这个该死的分类吗💢💥");
            if (confirmed)
            {
                var response = await HttpClient.DeleteAsync($"Category/DeleteCategory?categoryId={id}");
                if (response.IsSuccessStatusCode)
                {
                    categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
                }
            }
        }

        protected void Close(MouseEventArgs e) => Open = false;

        protected void UpdateCategory(CategoryDTO categoryDTO)
        {
            UploadData = categoryDTO;
            OnConfirm = UpdateSubmit;
            Open = true;
        }

        private async void UpdateSubmit(MouseEventArgs e)
        {
            var response = await HttpClient.PutAsJsonAsync("Category/UpdateCategory", UploadData);
            categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
            Open = false;
            StateHasChanged(); // 因为是 async void，需要通知 UI 刷新
        }

        protected void AddCategory()
        {
            UploadData = new CategoryDTO(); // 清空旧数据
            OnConfirm = AddSubmit;
            Open = true;
        }

        private async void AddSubmit(MouseEventArgs e)
        {
            var response = await HttpClient.PostAsJsonAsync("Category/AddCategory", UploadData);
            categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
            Open = false;
            StateHasChanged();
        }
    }
}