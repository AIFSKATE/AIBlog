using Blazor.Dialog;
using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminPosts
    {
        [Parameter] public int? page { get; set; }

        private int Limit = 15;
        private int TotalPage;
        private QueryPostsDto? PostsData;
        private int _isDeletedFilter = 0;

        protected override async Task OnParametersSetAsync()
        {
            // 当 URL 中的 page 参数改变时触发数据刷新
            await RenderPage(page ?? 1);
        }

        private async Task OnPageChanged(int newPage)
        {
            // 改变页面时，同步更新浏览器 URL，触发 OnParametersSetAsync
            Utils.NavigateTo($"/admin/posts/{newPage}");
        }

        private async Task OnFilterChanged(int value)
        {
            _isDeletedFilter = value;
            // 切换状态时重置回第一页，强制刷新
            if (page == 1 || page == null)
            {
                await RenderPage(1);
            }
            else
            {
                Utils.NavigateTo($"/admin/posts/1");
            }
        }

        private async Task RenderPage(int currentPage)
        {
            PostsData = null;
            try
            {
                var requestUrl = $"Post/AdminQueryPosts?Page={currentPage}&Limit={Limit}&isDeleted={_isDeletedFilter}";
                var result = await HttpClient.GetFromJsonAsync<QueryPostsDto>(requestUrl);

                if (result != null)
                {
                    PostsData = result;
                    TotalPage = (int)Math.Ceiling(PostsData.Count / (double)Limit);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("获取数据失败，请检查后端服务", Severity.Error);
            }
        }

        protected async Task OpenDeleteDialog(int id)
        {
            var parameters = new DialogParameters<GenericConfirmDialog>
            {
                { x => x.Title, "确认删除" },
                { x => x.ContentText, "确定要将这篇文章移入回收站吗?" },
                { x => x.Color, Color.Error },
                { x => x.Icon, Icons.Material.Outlined.DeleteForever },
                { x => x.OnConfirm, async (e) => await ExecuteDelete(id) }
            };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            await DialogService.ShowAsync<GenericConfirmDialog>("Delete", parameters, options);
        }

        private async Task ExecuteDelete(int id)
        {
            // 严格匹配后端路由: Post/DeletePost?postId=xxx
            var response = await HttpClient.DeleteAsync($"Post/DeletePost?postId={id}");

            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("文章已删除", Severity.Success);
                // 刷新当前页面数据
                await RenderPage(page ?? 1);
                StateHasChanged();
            }
            else
            {
                Snackbar.Add("操作失败，请检查权限或后端逻辑", Severity.Error);
            }
        }

        private async Task ExecuteRestore(int id)
        {
            // 调用后端的 RestorePost 接口
            var response = await HttpClient.PutAsync($"Post/RestorePost/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("文章已从回收站还原", Severity.Success);

                // 重新获取当前页数据
                await RenderPage(page ?? 1);

                // 手动触发 UI 刷新
                StateHasChanged();
            }
            else
            {
                Snackbar.Add("还原失败", Severity.Error);
            }
        }
    }
}