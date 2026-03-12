using Blazor.Dialog;
using Mapper.DTO;
using MudBlazor;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminFriendLink
    {
        private List<FriendLinkDTO> FriendLinks;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private string EnsureAbsoluteUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return "#";
            return url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : $"https://{url}";
        }

        private async Task LoadDataAsync()
        {
            // 对接 [HttpGet] QueryFriendLinks
            FriendLinks = await HttpClient.GetFromJsonAsync<List<FriendLinkDTO>>("FriendLink/QueryFriendLinks");
        }

        protected async Task OpenDeleteDialog(int id)
        {
            var parameters = new DialogParameters<GenericConfirmDialog>
            {
                { x => x.Title, "确认删除" },
                { x => x.ContentText, "确定要删除该友链吗？" },
                { x => x.Color, Color.Error },
                { x => x.Icon, Icons.Material.Outlined.DeleteForever },
                { x => x.OnConfirm, async (e) => await ExecuteDelete(id) }
            };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small,
            };
            await DialogService.ShowAsync<GenericConfirmDialog>("Delete", parameters, options);
        }

        private async Task ExecuteDelete(int id)
        {
            // 对接 [HttpDelete] DeleteFriendLink?friendLinkId=xx
            var response = await HttpClient.DeleteAsync($"FriendLink/DeleteFriendLink?friendLinkId={id}");
            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("友链已删除", Severity.Success);
                await LoadDataAsync();
                StateHasChanged();
            }
            else
            {
                Snackbar.Add("删除失败", Severity.Error);
            }
        }

        protected async Task OpenEditDialog(FriendLinkDTO item = null)
        {
            var isEdit = item != null;
            var model = isEdit ? new FriendLinkDTO { Id = item.Id, Title = item.Title, LinkUrl = item.LinkUrl } : new FriendLinkDTO();

            var parameters = new DialogParameters<FriendLinkEditDialog>
            {
                { x => x.Title, isEdit ? "修改友链信息" : "添加新友链" },
                { x => x.Model, model },
                { x => x.Color, Color.Primary }
            };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                FullWidth = true,
            };
            var dialog = await DialogService.ShowAsync<FriendLinkEditDialog>(isEdit ? "Edit" : "Add", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled && result.Data is FriendLinkDTO updatedModel)
            {
                await ExecuteSubmit(updatedModel, isEdit);
            }
        }

        private async Task ExecuteSubmit(FriendLinkDTO model, bool isEdit)
        {
            HttpResponseMessage response;
            if (isEdit)
            {
                // 对接 [HttpPut] UpdateFriendLink
                response = await HttpClient.PutAsJsonAsync("FriendLink/UpdateFriendLink", model);
            }
            else
            {
                // 对接 [HttpPost] AddLink (使用后端期望的 FriendLinkCreation 结构)
                var creationDto = new { Title = model.Title, LinkUrl = model.LinkUrl };
                response = await HttpClient.PostAsJsonAsync("FriendLink/AddLink", creationDto);
            }

            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add(isEdit ? "更新成功" : "添加成功", Severity.Success);
                await LoadDataAsync();
                StateHasChanged();
            }
            else
            {
                Snackbar.Add("操作失败，请检查数据", Severity.Error);
            }
        }
    }
}