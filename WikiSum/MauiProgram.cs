using DotNetEnv;
using Microsoft.Extensions.Logging;
using WikiSum.Services;
using WikiSum.Services.Translation;
using WikiSum.ViewModels;
namespace WikiSum
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
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
            Env.Load();
            builder.Services.AddTransient<ITranslationService, TranslationService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<IMainViewModel, MainViewModel>();
            return builder.Build();
        }
    }
}
