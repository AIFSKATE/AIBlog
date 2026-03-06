using Mapper.DTO;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class AdminTags
    {
        List<TagDTO> Tags;
        bool Open { get; set; } = false;
        TagDTO UploadData = new();

        // 使用 Action 替代带参数的鼠标事件，更简洁
        public Action OnConfirm { get; set; }
        public Action OnCancel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            OnCancel = Close;
            Tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
        }

        protected async Task DeleteTagAsync(int id)
        {
            bool confirmed = await Utils.InvokeAsync<bool>("confirm", "\n💥💢真的要干掉这个该死的分类吗💢💥");
            if (confirmed)
            {
                var response = await HttpClient.DeleteAsync($"Tag/DeleteTag?tagId={id}");
                if (response.IsSuccessStatusCode)
                {
                    Tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
                }
            }
        }

        protected void Close()
        {
            Open = false;
        }

        protected void UpdateTag(TagDTO tagDTO)
        {
            Open = true;
            UploadData = tagDTO;
            OnConfirm = UpdateSubmit;
        }

        private async void UpdateSubmit()
        {
            var response = await HttpClient.PutAsJsonAsync("Tag/UpdateTag", UploadData);
            Tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
            Close();
            StateHasChanged();
        }

        protected void AddTag()
        {
            Open = true;
            UploadData = new TagDTO(); // 清空旧数据
            OnConfirm = AddSubmit;
        }

        private async void AddSubmit()
        {
            var response = await HttpClient.PostAsJsonAsync("Tag/AddTag", UploadData.TagName);
            Tags = await HttpClient.GetFromJsonAsync<List<TagDTO>>("Tag/QueryAllTags");
            Close();
            StateHasChanged();
        }
    }
}