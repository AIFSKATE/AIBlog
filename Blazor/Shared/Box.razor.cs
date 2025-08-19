using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor.Shared
{
    public partial class Box
    {

        /// <summary>
        /// 组件内容
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        [Parameter]
        public bool Open { get; set; } = true;

        [Parameter]
        public string ConfirmText { get; set; } = "确定";

        [Parameter]
        public string CancelText { get; set; } = "取消";

        [Parameter]
        public Action<MouseEventArgs> OnConfirm { get; set; }

        [Parameter]
        public Action<MouseEventArgs> OnCancel { get; set; }
    }
}
