using Domain.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class Auth
    {
        private LoginInfo loginModel = new();
        private bool ShowMessage;
        private string Message = "Login failed, please try again.";

        protected override async Task OnInitializedAsync()
        {
            // 检查是否已经登录
            var token = await Utils.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await HttpClient.GetAsync("Account/CurrentUser");
            if (response.IsSuccessStatusCode)
            {
                NavigationManager.NavigateTo("admin");
                return;
            }
            await ShowInfoAsync("Token已过期");
        }

        private async Task HandleLoginAsync()
        {
            var response = await HttpClient.PostAsJsonAsync("/Account/Login", loginModel);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    await ShowInfoAsync("用户不存在");
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    await ShowInfoAsync("用户名或密码错误");
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    await ShowInfoAsync("账号已被锁定");
                    break;
                case System.Net.HttpStatusCode.OK:
                    await LoginSuccessAsync(response);
                    break;
                default:
                    await ShowInfoAsync("发生未知错误");
                    break;
            }
        }

        private async Task LoginSuccessAsync(HttpResponseMessage response)
        {
            var token = await response.Content.ReadAsStringAsync();
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine("登陆成功");
            await Utils.SetTokenAsync(token);
            NavigationManager.NavigateTo("admin");
        }

        private async Task ShowInfoAsync(string str)
        {
            Message = str;
            // 假设登录失败
            ShowMessage = true;
            await InvokeAsync(StateHasChanged);

            // 3 秒后自动隐藏
            await Task.Delay(3000);
            ShowMessage = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}
