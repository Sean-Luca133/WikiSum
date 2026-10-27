using DotNetEnv;
using Microsoft.Extensions.Logging;
using WikiSum.Services.Scrapping;
using WikiSum.ViewModels;
namespace WikiSum
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Env.Load();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddHttpClient<IScrapeService, ScrapeService>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<IMainViewModel, MainViewModel>();

            return builder.Build();
        }
    }
}
