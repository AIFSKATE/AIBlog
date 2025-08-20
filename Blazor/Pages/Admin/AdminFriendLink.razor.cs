using Domain.Post;
using Mapper.DTO;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminFriendLink
    {
        List<FriendLinkDTO> FriendLinks;
        bool Open { get; set; } = false;
        FriendLinkDTO UploadData = new();
        public Action<MouseEventArgs> OnConfirm { get; set; }
        public Action<MouseEventArgs> OnCancel { get; set; }
        bool ShowMessage { get; set; } = false;
        string Message { get; set; } = "发生错误";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            OnCancel = Close;
            FriendLinks = await HttpClient.GetFromJsonAsync<List<FriendLinkDTO>>("FriendLink/QueryFriendLinks");
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task DeleteFriendlinkAsync(int id)
        {
            // 弹窗确认
            bool confirmed = await Utils.InvokeAsync<bool>("confirm", "\n💥💢请问是否确定删除💢💥");

            if (confirmed)
            {
                var response = await HttpClient.DeleteAsync($"DeleteFriendLink?friendLinkId={id}");

                if (response.IsSuccessStatusCode)
                {
                    FriendLinks = await HttpClient.GetFromJsonAsync<List<FriendLinkDTO>>("FriendLink/QueryFriendLinks");
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        protected async void Close(MouseEventArgs e)
        {
            Open = false;
        }

        protected async Task UpdateFriendlink(FriendLinkDTO tagDTO)
        {
            Open = true;
            UploadData = tagDTO;
            OnConfirm = UpdateSubmit;
        }

        private async void UpdateSubmit(MouseEventArgs e)
        {
            var response = await HttpClient.PutAsJsonAsync<FriendLinkDTO>("FriendLink/UpdateFriendLink", UploadData);
            if (response.IsSuccessStatusCode)
            {
            }
            FriendLinks = await HttpClient.GetFromJsonAsync<List<FriendLinkDTO>>("FriendLink/QueryFriendLinks");
            Close(e);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task AddFriendlink()
        {
            Open = true;
            OnConfirm = AddSubmit;
        }

        private async void AddSubmit(MouseEventArgs e)
        {
            var response = await HttpClient.PostAsJsonAsync("FriendLink/AddLink", new FriendLinkCreation(UploadData.Title, UploadData.LinkUrl));
            if (response.IsSuccessStatusCode)
            {
            }
            FriendLinks = await HttpClient.GetFromJsonAsync<List<FriendLinkDTO>>("FriendLink/QueryFriendLinks");
            Close(e);
            await InvokeAsync(StateHasChanged);
        }
    }
}
