using Mapper.DTO;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminTags
    {
        List<TagDTO> Tags;
        bool Open { get; set; } = false;
        TagDTO UploadData = new();
        public Action<MouseEventArgs> OnConfirm { get; set; }
        public Action<MouseEventArgs> OnCancel { get; set; }
        bool ShowMessage { get; set; } = false;
        string Message { get; set; } = "发生错误";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            OnCancel = Close;
            Tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task DeleteTagAsync(int id)
        {
            // 弹窗确认
            bool confirmed = await Utils.InvokeAsync<bool>("confirm", "\n💥💢真的要干掉这个该死的分类吗💢💥");

            if (confirmed)
            {
                var response = await HttpClient.DeleteAsync($"Tag/DeleteTag?tagId={id}");

                if (response.IsSuccessStatusCode)
                {
                    Tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        protected async void Close(MouseEventArgs e)
        {
            Open = false;
        }

        protected async Task UpdateTag(TagDTO tagDTO)
        {
            Open = true;
            UploadData = tagDTO;
            OnConfirm = UpdateSubmit;
        }

        private async void UpdateSubmit(MouseEventArgs e)
        {
            var response = await HttpClient.PutAsJsonAsync<TagDTO>("Tag/UpdateTag", UploadData);
            if (response.IsSuccessStatusCode)
            {
            }
            Tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
            Close(e);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task AddTag()
        {
            Open = true;
            OnConfirm = AddSubmit;
        }

        private async void AddSubmit(MouseEventArgs e)
        {
            Console.WriteLine(UploadData.TagName);
            var response = await HttpClient.PostAsJsonAsync("Tag/AddTag", UploadData.TagName);
            if (response.IsSuccessStatusCode)
            {
            }
            Tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
            Close(e);
            await InvokeAsync(StateHasChanged);
        }
    }
}
