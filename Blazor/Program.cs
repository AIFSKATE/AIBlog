using Blazor.Commons;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var baseAddress = "https://localhost:7218/";
            if (builder.HostEnvironment.IsProduction())
            {
                baseAddress = "https://localhost:7218/";
            }

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
            builder.Services.AddSingleton<Utils>();

            await builder.Build().RunAsync();
        }
    }
}
