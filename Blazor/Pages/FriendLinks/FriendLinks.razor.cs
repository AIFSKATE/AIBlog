using Mapper.DTO;
using System.Net.Http.Json;

namespace Blazor.Pages.FriendLinks
{
    public partial class FriendLinks
    {
        private List<FriendLinkDTO> friendLinks;

        protected override async Task OnInitializedAsync()
        {
            // 获取数据，全局 HttpClient 发起请求
            friendLinks = await HttpClient.GetFromJsonAsync<List<FriendLinkDTO>>("FriendLink/QueryFriendLinks");
        }
    }
}