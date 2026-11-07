using DotNetEnv;
using Microsoft.Extensions.Logging;
using WikiSum.Services;
using WikiSum.Services.API;
using WikiSum.Services.LanguageDetection;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;
using WikiSum.ViewModels;
using WikiSum.Views;
using WikiSum.Services.LanguageDetection;
using WikiSum.Services.EntityReconition;
using WikiSum.Services.Navigation;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;

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
            builder.Services.AddSingleton<IEntityRecognitionService, EntityRecognitionService>();
            builder.Services.AddTransient<ILanguageDetectionService, LanguageDetectionService>();
            builder.Services.AddHttpClient<IScrapeService, ScrapeService>();
            builder.Services.AddSingleton<ILanguageDetectionService, LanguageDetectionService>();
            builder.Services.AddSingleton<ITranslationService, TranslationService>();
            builder.Services.AddSingleton<ISummarizationService, SummarizationService>();
            builder.Services.AddHttpClient<IAPIService, ApiService>();
            builder.Services.AddTransient<INavigationService, NavigationService>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<IMainViewModel, MainViewModel>();

            builder.Services.AddTransient<SummaryPage>();
            builder.Services.AddTransient<ISummaryViewModel, SummaryViewModel>();

            builder.Services.AddTransient<SummaryDetailPage>();
            builder.Services.AddTransient<ISummaryDetailViewModel, SummaryDetailViewModel>();

            builder.Services.AddTransient<WebsiteSummaryPage>();
            builder.Services.AddTransient<IWebsiteSummaryViewModel, WebsiteSummaryViewModel>();

            builder.Services.AddTransient<EntityPopup>();
            builder.Services.AddTransient<EntityPopupViewModel>();
            builder.Services.AddTransient<IPopupService, PopupService>();
            builder.Services.AddTransientPopup<EntityPopup, EntityPopupViewModel>();

            builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit();
            return builder.Build();
        }
    }
}
