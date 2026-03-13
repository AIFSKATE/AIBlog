using Blazor.Commons;
using Domain.Account;
using MudBlazor;
using System.Net.Http.Json;

namespace Blazor.Pages.Admin
{
    public partial class Auth
    {
        private LoginInfo loginModel = new();

        protected override async Task OnInitializedAsync()
        {
            var token = await Utils.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await HttpClient.GetAsync("Account/CurrentUser");
            if (response.IsSuccessStatusCode)
            {
                await Utils.NavigateTo("admin");
                return;
            }

            // Replaced manual method with MudBlazor Snackbar
            Snackbar.Add("Token已过期，请重新登录", Severity.Warning);
        }

        private async Task HandleLoginAsync()
        {
            var response = await HttpClient.PostAsJsonAsync("Account/Login", loginModel);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    Snackbar.Add("用户不存在", Severity.Error);
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    Snackbar.Add("用户名或密码错误", Severity.Error);
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    Snackbar.Add("账号已被锁定", Severity.Error);
                    break;
                case System.Net.HttpStatusCode.OK:
                    await LoginSuccessAsync(response);
                    break;
                default:
                    Snackbar.Add("发生未知错误", Severity.Error);
                    break;
            }
        }

        private async Task LoginSuccessAsync(HttpResponseMessage response)
        {
            var token = await response.Content.ReadAsStringAsync();
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await Utils.SetTokenAsync(token);
            (AuthStateProvider as AIBlogAuthStateProvider)?.NotifyUserAuthentication();

            Snackbar.Add("登录成功", Severity.Success);
            await Utils.NavigateTo("admin");
        }

    }
}