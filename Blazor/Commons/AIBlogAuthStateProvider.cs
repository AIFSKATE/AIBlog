namespace Blazor.Commons
{
    using Domain.Account;
    using Microsoft.AspNetCore.Components.Authorization;
    using System.Net.Http.Json;
    using System.Security.Claims;

    public class AIBlogAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;
        private readonly Blazor.Commons.Utils utils;
        private readonly AuthenticationState anonymous;

        public AIBlogAuthStateProvider(HttpClient httpClient, Blazor.Commons.Utils utils)
        {
            this.httpClient = httpClient;
            this.utils = utils;
            anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // 1. 调用你的 Utils 获取本地存储的 Token
            var token = await utils.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return anonymous;
            }

            // 2. 配置 HttpClient 默认请求头，这样后续所有请求都自动带 Token
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                // 3. 调用你现有的后端接口获取当前用户信息
                var response = await httpClient.GetAsync("Account/CurrentUser");
                if (response.IsSuccessStatusCode)
                {
                    var userInfo = await response.Content.ReadFromJsonAsync<CurrentUserInfo>();
                    if (userInfo != null)
                    {
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                        new Claim(ClaimTypes.Name, userInfo.UserName)
                    };

                        // 将角色列表全部注入，以便使用 [Authorize(Roles="Admin")]
                        claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

                        var identity = new ClaimsIdentity(claims, "AIBlogAuth");
                        return new AuthenticationState(new ClaimsPrincipal(identity));
                    }
                }
            }
            catch (Exception ex)
            {
                return anonymous;
            }
            return anonymous;
        }

        // 当用户登录或退出时，手动调用此方法来通知全站刷新 UI
        public void NotifyUserAuthentication()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
