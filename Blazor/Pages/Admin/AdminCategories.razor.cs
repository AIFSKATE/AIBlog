using Domain.Post;
using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;

namespace Blazor.Pages.Admin
{
    public partial class AdminCategories
    {
        List<CategoryDTO> categories;
        bool Open { get; set; } = false;
        CategoryDTO UploadData = new();
        public Action<MouseEventArgs> OnConfirm { get; set; }
        public Action<MouseEventArgs> OnCancel { get; set; }
        bool ShowMessage { get; set; } = false;
        string Message { get; set; } = "发生错误";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            OnCancel = Close;
            categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task DeleteAsync(int id)
        {
            // 弹窗确认
            bool confirmed = await Utils.InvokeAsync<bool>("confirm", "\n💥💢真的要干掉这个该死的分类吗💢💥");

            if (confirmed)
            {
                var response = await HttpClient.DeleteAsync($"Category/DeleteCategory?categoryId={id}");

                if (response.IsSuccessStatusCode)
                {
                    categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
                }
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async void Close(MouseEventArgs e)
        {
            Open = false;
        }

        protected async Task UpdateCategory(CategoryDTO categoryDTO)
        {
            Open = true;
            UploadData = categoryDTO;
            OnConfirm = UpdateSubmit;
        }

        private async void UpdateSubmit(MouseEventArgs e)
        {
            var response = await HttpClient.PutAsJsonAsync<CategoryDTO>("Category/UpdateCategory", UploadData);
            if (response.IsSuccessStatusCode)
            {
            }
            categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
            Close(e);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task AddCategory()
        {
            Open = true;
            OnConfirm = AddSubmit;
        }

        private async void AddSubmit(MouseEventArgs e)
        {
            var response = await HttpClient.PostAsJsonAsync<CategoryDTO>("Category/AddCategory", UploadData);
            if (response.IsSuccessStatusCode)
            {
            }
            categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>("Category/QueryCategories");
            Close(e);
            await InvokeAsync(StateHasChanged);
        }
    }
}
