using EFCore.Data;
using Mapper.DTO;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Blazor.Pages.Posts
{
    public partial class Post
    {
        [Parameter]
        public int Id { get; set; }

        PostDTO? PostDto { get; set; }

        public bool Error = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var response = await HttpClient.GetAsync($"/Post/GetPost?postId={Id}");

                response.EnsureSuccessStatusCode();

                PostDto = await response.Content.ReadFromJsonAsync<PostDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"未知异常: {ex.Message}");
                Error = true;
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}