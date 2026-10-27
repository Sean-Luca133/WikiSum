using DotNetEnv;
using Microsoft.Extensions.Logging;
<<<<<<< HEAD
using WikiSum.Services;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;
using WikiSum.ViewModels;
=======
using WikiSum.Services.Scrapping;
>>>>>>> 0b2ba4b1009f25f5b36eea0bd868fd7470fd6f1e
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
<<<<<<< HEAD
            Env.Load();
            builder.Services.AddTransient<ITranslationService, TranslationService>();
            builder.Services.AddTransient<ISummarizationService, SummarizationService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<IMainViewModel, MainViewModel>();
=======
            builder.Services.AddHttpClient<IScrapeService, ScrapeService>();

>>>>>>> 0b2ba4b1009f25f5b36eea0bd868fd7470fd6f1e
            return builder.Build();
        }
    }
}
