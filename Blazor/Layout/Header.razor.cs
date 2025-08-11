
using Blazor.Commons;

namespace Blazor.Layout
{
    public partial class Header
    {
        /// <summary>
        /// 下拉菜单是否打开
        /// </summary>
        private bool collapseNavMenu = false;

        /// <summary>
        /// 导航菜单CSS
        /// </summary>
        private string NavMenuCssClass => collapseNavMenu ? "active" : null;

        /// <summary>
        /// 显示/隐藏 菜单
        /// </summary>
        private void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;

        /// <summary>
        /// 当前主题
        /// </summary>
        private string currentTheme = string.Empty;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            currentTheme = await Utils.GetStorageAsync("theme") ?? "Light";

            await Utils.InvokeAsync("window.func.switchTheme");
        }

        /// <summary>
        /// 切换主题
        /// </summary>
        private async Task SwitchTheme()
        {
            currentTheme = currentTheme == "Light" ? "Dark" : "Light";

            await Utils.SetStorageAsync("theme", currentTheme);

            await Utils.InvokeAsync("window.func.switchTheme");
        }
    }
}
