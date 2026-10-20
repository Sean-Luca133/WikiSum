using DotNetEnv;
using EchoTranslate.Services;
using Microsoft.Extensions.Logging;
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
            Console.WriteLine(TranslationService.TranslateText("How are You?","en","nl"));
            return builder.Build();
        }
    }
}
