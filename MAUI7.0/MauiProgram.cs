using MAUI7._0.Data;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BackuperGUI.Data;

namespace MAUI7._0
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            }).UseMauiCommunityToolkit();
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSingleton<BackuperInMaui>();
            return builder.Build();
        }
    }
}