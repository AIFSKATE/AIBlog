using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blazor.Shared
{
    public partial class Chips
    {
        [Parameter]
        public List<(bool selected, string name)> Items { get; set; } = new();

        [Parameter]
        public bool IsCheckBox { get; set; } = true;

        private async Task ToggleCheckBox(int index)
        {
            var item = Items[index];
            Items[index] = (!item.selected, item.name);
            await InvokeAsync(StateHasChanged);
        }

        private async Task ToggleRadioBox(int index)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i] = (false, Items[i].name);
            }
            var item = Items[index];
            Items[index] = (true, item.name);
            await InvokeAsync(StateHasChanged);
        }
    }
}
