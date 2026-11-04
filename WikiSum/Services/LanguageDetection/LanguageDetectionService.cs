using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services.LanguageDetection
{
    public class LanguageDetectionService : ILanguageDetectionService
    {


        public static class ApiLanguagenKeys
        {
            public static string LanguagerKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
            public static string LanguageEndpoint = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
        }

        private TextAnalyticsClient client = new(new Uri(ApiLanguagenKeys.LanguageEndpoint), new AzureKeyCredential(ApiLanguagenKeys.LanguagerKey));


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
