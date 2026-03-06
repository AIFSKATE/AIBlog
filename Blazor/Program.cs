using Blazor.Commons;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var baseAddress = builder.Configuration.GetValue<string>("BaseAddress") ?? "https://aifskate.com/api/";

            builder.Services.AddMudServices();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
            builder.Services.AddSingleton<Utils>();

            await builder.Build().RunAsync();
        }
    }
}
