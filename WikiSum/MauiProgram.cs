using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WikiSum.Services;
using WikiSum.Services.API;
using WikiSum.Services.EntityReconition;
using WikiSum.Services.LanguageDetection;
using WikiSum.Services.Navigation;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;
using WikiSum.ViewModels;
using WikiSum.Views;

namespace WikiSum
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();

            // Laad appsettings.json via MAUI Essentials FileSystem
            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

                // Koppel alle instellingen aan Environment variables zodat ApiLanguageKeys ze pakt
                Settings.LanguageKey = config["LANGUAGE_KEY"];
                Settings.LanguageEndpoint = config["LANGUAGE_ENDPOINT"];
                Settings.TranslatorKey = config["TRANSLATOR_KEY"];
                Settings.TranslatorRegion = config["TRANSLATOR_REGION"];
                Settings.TranslatorEndpoint = config["TRANSLATOR_ENDPOINT"];
                Settings.BackendApiEndpoint = config["BACKEND_API_ENDPOINT"];

                builder.Configuration.AddConfiguration(config);
                Debug.WriteLine($"Succesvol geladen van de appsettings.json");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fout bij laden van appsettings.json: {ex.Message}");
            }

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Services registreren (dubbele ILanguageDetectionService verwijderd)
            builder.Services.AddSingleton<IEntityRecognitionService, EntityRecognitionService>();
            builder.Services.AddSingleton<ILanguageDetectionService, LanguageDetectionService>();
            builder.Services.AddHttpClient<IScrapeService, ScrapeService>();
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

            // Alles correct geconfigureerd aan het einde
            return builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit()
                .Build();
        }
    }
}