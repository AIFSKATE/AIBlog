using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.Commons
{
    public class Utils
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;

        public Utils(IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        /// <summary>
        /// 执行无返回值方法
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async ValueTask InvokeAsync(string identifier, params object[] args)
        {
            await _jsRuntime.InvokeVoidAsync(identifier, args);
        }

        /// <summary>
        /// 执行带返回值的方法
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, params object[] args)
        {
            return await _jsRuntime.InvokeAsync<TValue>(identifier, args);
        }

        /// <summary>
        /// 设置localStorage
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetStorageAsync(string name, string value)
        {
            await InvokeAsync("window.func.setStorage", name, value);
        }

        /// <summary>
        /// 获取localStorage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<string> GetStorageAsync(string name)
        {
            return await InvokeAsync<string>("window.func.getStorage", name);
        }

        /// <summary>
        /// 跳转指定URL
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="forceLoad">
        /// true:强制刷新刷新页面
        /// false:浏览器不刷新
        /// Blazor掐指一算,发现新页面对应的组件是B，于是卸载A，挂载B。</param>
        /// <returns></returns>
        public async Task NavigateTo(string url, bool forceLoad = false)
        {
            _navigationManager.NavigateTo(url, forceLoad);

            await Task.CompletedTask;
        }


        /// <summary>
        /// 后退
        /// </summary>
        /// <returns></returns>
        public async Task BaskAsync()
        {
            await InvokeAsync("window.history.back");
        }

        /// <summary>
        /// 清楚Storage
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal async Task ClearStorageAsync()
        {
            await InvokeAsync("window.func.clearAllStorage");
        }

        internal async Task SetTokenAsync(string token)
        {
            await SetStorageAsync("aiblog_token", token);
        }

        internal async Task<string> GetTokenAsync()
        {
            return await GetStorageAsync("aiblog_token");
        }
    }
}
