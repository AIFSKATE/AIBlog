using Blazor.Commons;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
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

            // Program.cs
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;

                config.SnackbarConfiguration.NewestOnTop = true;

                config.SnackbarConfiguration.MaxDisplayedSnackbars = 10;

                config.SnackbarConfiguration.VisibleStateDuration = 1500;

                config.SnackbarConfiguration.HideTransitionDuration = 200;

                config.SnackbarConfiguration.ShowTransitionDuration = 200;

                config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;

                config.SnackbarConfiguration.PreventDuplicates = false;

                config.SnackbarConfiguration.BackgroundBlurred = true;
            });
            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<AuthenticationStateProvider, AIBlogAuthStateProvider>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
            builder.Services.AddSingleton<Utils>();
            builder.Services.AddSingleton<MarkdownService>();

            await builder.Build().RunAsync();
        }
    }
}
