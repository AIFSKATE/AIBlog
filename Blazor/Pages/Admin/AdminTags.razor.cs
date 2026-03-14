using Blazor.Dialog;
using Mapper.DTO;
using MudBlazor;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminTags
    {
        private List<TagDTO>? tags;

        protected override async Task OnInitializedAsync()
        {
            await LoadTags();
        }

        private async Task LoadTags()
        {
            tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
        }

        protected async Task OpenDeleteDialog(TagDTO tag)
        {
            var parameters = new DialogParameters<GenericConfirmDialog>
            {
                { x => x.Title, "确认删除" },
                { x => x.ContentText, $"确定要彻底干掉标签 [{tag.TagName}] 吗？关联关系将自动解除。" },
                { x => x.Color, Color.Error },
                { x => x.Icon, Icons.Material.Outlined.DeleteForever },
                { x => x.OnConfirm, async (e) => { await ExecuteDelete(tag.Id); } }
            };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
                MaxWidth = MaxWidth.Small
            };

            await DialogService.ShowAsync<GenericConfirmDialog>("Delete", parameters, options);
        }

        private async Task ExecuteDelete(int id)
        {
            var response = await HttpClient.DeleteAsync($"Tag/DeleteTag?tagId={id}");
            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("标签已彻底删除", Severity.Success);
                await LoadTags();
                StateHasChanged();
            }
        }

        private async Task OpenTagDialogAsync(TagDTO model, string title, bool isUpdate)
        {
            var parameters = new DialogParameters<TagEditDialog>
            {
                { x => x.Title, title },
                { x => x.Color, Color.Primary },
                { x => x.Model, model },
             };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
                MaxWidth = MaxWidth.Small
            };

            var dialog = await DialogService.ShowAsync<TagEditDialog>(isUpdate ? "Edit" : "Add", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var tagData = result.Data as TagDTO;

                // 核心检查：如果 TagName 为空或仅包含空格，直接报错并返回
                if (tagData != null && string.IsNullOrWhiteSpace(tagData.TagName))
                {
                    Snackbar.Add("标签名称不能为空", Severity.Error);
                    return;
                }

                var response = isUpdate
                    ? await HttpClient.PutAsJsonAsync("Tag/UpdateTag", tagData)
                    : await HttpClient.PostAsJsonAsync("Tag/AddTag", tagData.TagName);

                if (response.IsSuccessStatusCode)
                {
                    Snackbar.Add(isUpdate ? "更新成功" : "新增成功", Severity.Success);
                    await LoadTags();
                }
                else
                {
                    // 读取后端返回的错误信息（比如：This tag already exists）
                    var error = await response.Content.ReadAsStringAsync();
                    Snackbar.Add(error ?? "操作失败", Severity.Error);
                }
            }
        }

        protected async Task AddTag()
        {
            await OpenTagDialogAsync(new TagDTO(), "新增标签", false);
        }

        protected async Task UpdateTag(TagDTO tag)
        {
            var modelCopy = new TagDTO
            {
                Id = tag.Id,
                TagName = tag.TagName
            };
            await OpenTagDialogAsync(modelCopy, "编辑标签", true);
        }
    }
}