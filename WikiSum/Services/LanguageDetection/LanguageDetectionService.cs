using Azure;
using Azure.AI.TextAnalytics;

namespace WikiSum.Services.LanguageDetection
{
    public class LanguageDetectionService : ILanguageDetectionService
    {

        private TextAnalyticsClient client = new(new Uri(ApiLanguageKeys.LanguageEndpoint), new AzureKeyCredential(ApiLanguageKeys.LanguageKey));


        public async Task<DetectedLanguage?> DetectLanguageInput(string document)
        {
            try
            {
                Response<DetectedLanguage> response = await client.DetectLanguageAsync(document);
                DetectedLanguage language = response.Value;


                Console.WriteLine($"[LanguageService]: Detected language is {language.Name} with a confidence score of {language.ConfidenceScore}.");
                return language;

                
            }
            catch (RequestFailedException exception)
            {   
                Console.WriteLine($"[LanguageService]: Error Code: {exception.ErrorCode}");
                Console.WriteLine($"[LanguageService]: Message: {exception.Message}");
                return null;
                
            }
        }
    }
}
