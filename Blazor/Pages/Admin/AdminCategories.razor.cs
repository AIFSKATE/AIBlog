using Blazor.Dialog;
using Mapper.DTO;
using MudBlazor;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminCategories
    {
        private List<CategoryDTO>? categories;
        private int _isDeletedFilter = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadCategories();
        }

        private async Task LoadCategories()
        {
            categories = null;
            try
            {
                var requestUrl = $"Category/AdminQueryCategories?isDeleted={_isDeletedFilter}";
                categories = await HttpClient.GetFromJsonAsync<List<CategoryDTO>>(requestUrl);
            }
            catch (Exception ex)
            {
                Snackbar.Add("获取分类列表失败", Severity.Error);
            }
        }

        private async Task OnFilterChanged(int value)
        {
            _isDeletedFilter = value;
            await LoadCategories();
            StateHasChanged();
        }

        protected async Task OpenDeleteDialog(CategoryDTO category)
        {
            var parameters = new DialogParameters<GenericConfirmDialog>
            {
                { x => x.Title, "确认删除" },
                { x => x.ContentText, $"确定要干掉分类 {category.CategoryName} 吗？注意：旗下文章也将被隐藏。" },
                { x => x.Color, Color.Error },
                { x => x.Icon, Icons.Material.Outlined.DeleteForever },
                { x => x.OnConfirm, async (e) => { await ExecuteDelete(category.Id); } }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Small };
            await DialogService.ShowAsync<GenericConfirmDialog>("Delete", parameters, options);
        }

        private async Task ExecuteDelete(int id)
        {
            var response = await HttpClient.DeleteAsync($"Category/DeleteCategory?categoryId={id}");
            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("分类及其关联文章已移除", Severity.Success);
                await LoadCategories();
                StateHasChanged();
            }
        }

        private async Task ExecuteRestore(int id)
        {
            // Matching the Restore logic from AdminPosts
            var response = await HttpClient.PutAsync($"Category/RestoreCategory?categoryId={id}", null);
            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("分类及其关联文章已恢复", Severity.Success);
                await LoadCategories();
                StateHasChanged();
            }
        }

        private async Task OpenCategoryDialogAsync(CategoryDTO model, string title, bool isUpdate)
        {
            var parameters = new DialogParameters<CategoryEditDialog>
            {
                { x => x.Title, title },
                { x => x.Color, Color.Primary },
                { x => x.Model, model },
             };

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<CategoryEditDialog>(isUpdate ? "Edit" : "Add", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var response = isUpdate
                    ? await HttpClient.PutAsJsonAsync("Category/UpdateCategory", (CategoryDTO)result.Data)
                    : await HttpClient.PostAsJsonAsync("Category/AddCategory", (CategoryDTO)result.Data);

                if (response.IsSuccessStatusCode)
                {
                    Snackbar.Add(isUpdate ? "更新成功" : "新增成功", Severity.Success);
                    await LoadCategories();
                }
            }
        }

        protected async Task AddCategory()
        {
            await OpenCategoryDialogAsync(new CategoryDTO(), "新增分类", false);
        }

        protected async Task UpdateCategory(CategoryDTO category)
        {
            var modelCopy = new CategoryDTO
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
            await OpenCategoryDialogAsync(modelCopy, "编辑分类", true);
        }
    }
}