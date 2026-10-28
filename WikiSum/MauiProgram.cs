using DotNetEnv;
using Microsoft.Extensions.Logging;
using WikiSum.Services;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;
using WikiSum.ViewModels;
using WikiSum.Services.Scrapping;
using WikiSum.Views;

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
            Env.Load();
            builder.Services.AddHttpClient<IScrapeService, ScrapeService>();
            builder.Services.AddTransient<ITranslationService, TranslationService>();
            builder.Services.AddTransient<ISummarizationService, SummarizationService>();
          
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<IMainViewModel, MainViewModel>();

            builder.Services.AddTransient<SummaryPage>();
            builder.Services.AddTransient<ISummaryViewModel, SummaryViewModel>();


            return builder.Build();
        }
    }
}
