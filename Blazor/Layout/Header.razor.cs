using Blazor.Commons;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blazor.Layout
{
    public partial class Header
    {
        /// <summary>
        /// 移动菜单是否打开
        /// </summary>
        private bool mobileMenuOpen = false;

        /// <summary>
        /// 是否是暗黑主题
        /// </summary>
        private bool isDarkTheme = false;

        /// <summary>
        /// 切换移动菜单
        /// </summary>
        private void ToggleMobileMenu() => mobileMenuOpen = !mobileMenuOpen;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            var savedTheme = await Utils.GetStorageAsync("theme") ?? "Light";
            isDarkTheme = savedTheme == "Dark";

            await Utils.InvokeAsync("window.func.switchTheme");
        }

        /// <summary>
        /// 主题切换处理（由 @bind-Checked 自动处理）
        /// </summary>
        private async Task UpdateTheme()
        {
            var theme = isDarkTheme ? "Dark" : "Light";
            await Utils.SetStorageAsync("theme", theme);
            await Utils.InvokeAsync("window.func.switchTheme");
        }
    }
}
