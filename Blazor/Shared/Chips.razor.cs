using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blazor.Shared
{
    public enum ChipType
    {
        Checkbox,
        Radio,
        Show
    }

    public partial class Chips
    {
        [Parameter]
        public List<string> Items { get; set; } = new();

        [Parameter]
        public List<int> Selected { get; set; } = new();

        [Parameter]
        public ChipType Type { get; set; } = ChipType.Checkbox;

        public Func<int, Task>? OnClick { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            switch (Type)
            {
                case ChipType.Checkbox:
                    OnClick = ToggleCheckBox;
                    break;
                case ChipType.Radio:
                    OnClick = ToggleRadioBox;
                    break;
                case ChipType.Show:
                    OnClick = null;
                    break;
                default:
                    break;
            }
        }

        private async Task ToggleCheckBox(int index)
        {
            Selected[index] = Selected[index] ^ 1;
            await InvokeAsync(StateHasChanged);
        }

        private async Task ToggleRadioBox(int index)
        {
            if (Selected[index] == 0)
            {
                for (int i = 0; i < Selected.Count; i++)
                {
                    Selected[i] = 0;
                }
                Selected[index] = 1;
            }
            else
            {
                Selected[index] = 0;
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}
